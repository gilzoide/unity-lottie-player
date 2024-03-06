// This is a port of Earcut.NET that works with Unity collections
// Reference: https://github.com/oberbichler/earcut.net
// Earcut.NET license:
//
// ISC License
//
// Copyright (c) 2018, Thomas Oberbichler
//
// Permission to use, copy, modify, and/or distribute this software for any purpose
// with or without fee is hereby granted, provided that the above copyright notice
// and this permission notice appear in all copies.
//
// THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH
// REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT,
// INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS
// OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER
// TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF
// THIS SOFTWARE.

using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Gilzoide.LottiePlayer.Tessellation.EarcutNet
{
    public struct Earcut : IJob
    {
        public NativeSlice<float2> Data;
        public NativeList<int> OutTriangles;
        public int BaseVertex;

        public void Execute()
        {
            var outerLen = Data.Length;
            var outerNode = LinkedList(Data, 0, outerLen, true);

            if (outerNode == null)
            {
                return;
            }

            var min = new float2(float.PositiveInfinity);
            var invSize = default(float);
            // if the shape is not too simple, we'll use z-order curve hash later; calculate polygon bbox
            if (Data.Length > 80)
            {
                var max = new float2(float.NegativeInfinity);
                for (int i = 0; i < outerLen; i++)
                {
                    float2 point = Data[i];
                    min = math.min(min, point);
                    max = math.max(max, point);
                }

                // minX, minY and invSize are later used to transform coords into integers for z-order calculation
                float2 size = max - min;
                invSize = math.max(size.x, size.y);
                invSize = invSize != 0 ? 1 / invSize : 0;
            }

            EarcutLinked(outerNode, OutTriangles, min, invSize, 0);
        }

        // Creates a circular doubly linked list from polygon points in the specified winding order.
        static Node LinkedList(NativeSlice<float2> data, int start, int end, bool clockwise)
        {
            var last = default(Node);

            if (clockwise == (SignedArea(data, start, end) > 0))
            {
                for (int i = start; i < end; i++)
                {
                    last = InsertNode(i, data[i], last);
                }
            }
            else
            {
                for (int i = end - 1; i >= start; i--)
                {
                    last = InsertNode(i, data[i], last);
                }
            }

            if (last != null && Equals(last, last.next))
            {
                RemoveNode(last);
                last = last.next;
            }

            return last;
        }

        // eliminate colinear or duplicate points
        static Node FilterPoints(Node start, Node end = null)
        {
            if (start == null)
            {
                return start;
            }

            if (end == null)
            {
                end = start;
            }

            var p = start;
            bool again;

            do
            {
                again = false;

                if (Equals(p, p.next) || Area(p.prev, p, p.next) == 0)
                {
                    RemoveNode(p);
                    p = end = p.prev;
                    if (p == p.next)
                    {
                        break;
                    }

                    again = true;

                }
                else
                {
                    p = p.next;
                }
            } while (again || p != end);

            return end;
        }

        // main ear slicing loop which triangulates a polygon (given as a linked list)
        private readonly void EarcutLinked(Node ear, NativeList<int> triangles, float2 min, float invSize, int pass = 0)
        {
            if (ear == null)
            {
                return;
            }

            // interlink polygon nodes in z-order
            if (pass == 0 && invSize != 0)
            {
                IndexCurve(ear, min, invSize);
            }

            var stop = ear;
            Node prev;
            Node next;

            // iterate through ears, slicing them one by one
            while (ear.prev != ear.next)
            {
                prev = ear.prev;
                next = ear.next;

                if (invSize != 0 ? IsEarHashed(ear, min, invSize) : IsEar(ear))
                {
                    // cut off the triangle
                    triangles.Add(BaseVertex + prev.i);
                    triangles.Add(BaseVertex + ear.i);
                    triangles.Add(BaseVertex + next.i);

                    RemoveNode(ear);

                    // skipping the next vertex leads to less sliver triangles
                    ear = next.next;
                    stop = next.next;

                    continue;
                }

                ear = next;

                // if we looped through the whole remaining polygon and can't find any more ears
                if (ear == stop)
                {
                    // try filtering points and slicing again
                    if (pass == 0)
                    {
                        EarcutLinked(FilterPoints(ear), triangles, min, invSize, 1);

                        // if this didn't work, try curing all small self-intersections locally
                    }
                    else if (pass == 1)
                    {
                        ear = CureLocalIntersections(ear, triangles);
                        EarcutLinked(ear, triangles, min, invSize, 2);

                        // as a last resort, try splitting the remaining polygon into two
                    }
                    else if (pass == 2)
                    {
                        SplitEarcut(ear, triangles, min, invSize);
                    }

                    break;
                }
            }
        }

        // check whether a polygon node forms a valid ear with adjacent nodes
        static bool IsEar(Node ear)
        {
            var a = ear.prev;
            var b = ear;
            var c = ear.next;

            if (Area(a, b, c) >= 0)
            {
                return false; // reflex, can't be an ear
            }

            // now make sure we don't have other points inside the potential ear
            var p = ear.next.next;

            while (p != ear.prev)
            {
                if (PointInTriangle(a.x, a.y, b.x, b.y, c.x, c.y, p.x, p.y) &&
                    Area(p.prev, p, p.next) >= 0)
                {
                    return false;
                }

                p = p.next;
            }

            return true;
        }

        static bool IsEarHashed(Node ear, float2 min, float invSize)
        {
            var a = ear.prev;
            var b = ear;
            var c = ear.next;

            if (Area(a, b, c) >= 0)
            {
                return false; // reflex, can't be an ear
            }

            var minT = math.min(a.position, math.min(b.position, c.position));
            var maxT = math.max(a.position, math.max(b.position, c.position));

            // z-order range for the current triangle bbox;
            var minZ = ZOrder(minT, min, invSize);
            var maxZ = ZOrder(maxT, min, invSize);

            var p = ear.prevZ;
            var n = ear.nextZ;

            // look for points inside the triangle in both directions
            while (p != null && p.z >= minZ && n != null && n.z <= maxZ)
            {
                if (p != ear.prev && p != ear.next &&
                    PointInTriangle(a.x, a.y, b.x, b.y, c.x, c.y, p.x, p.y) &&
                    Area(p.prev, p, p.next) >= 0)
                {
                    return false;
                }

                p = p.prevZ;

                if (n != ear.prev && n != ear.next &&
                    PointInTriangle(a.x, a.y, b.x, b.y, c.x, c.y, n.x, n.y) &&
                    Area(n.prev, n, n.next) >= 0)
                {
                    return false;
                }

                n = n.nextZ;
            }

            // look for remaining points in decreasing z-order
            while (p != null && p.z >= minZ)
            {
                if (p != ear.prev && p != ear.next &&
                    PointInTriangle(a.x, a.y, b.x, b.y, c.x, c.y, p.x, p.y) &&
                    Area(p.prev, p, p.next) >= 0)
                {
                    return false;
                }

                p = p.prevZ;
            }

            // look for remaining points in increasing z-order
            while (n != null && n.z <= maxZ)
            {
                if (n != ear.prev && n != ear.next &&
                    PointInTriangle(a.x, a.y, b.x, b.y, c.x, c.y, n.x, n.y) &&
                    Area(n.prev, n, n.next) >= 0)
                {
                    return false;
                }

                n = n.nextZ;
            }

            return true;
        }

        // go through all polygon nodes and cure small local self-intersections
        private readonly Node CureLocalIntersections(Node start, NativeList<int> triangles)
        {
            var p = start;
            do
            {
                var a = p.prev;
                var b = p.next.next;

                if (!Equals(a, b) && Intersects(a, p, p.next, b) && LocallyInside(a, b) && LocallyInside(b, a))
                {

                    triangles.Add(BaseVertex + a.i);
                    triangles.Add(BaseVertex + p.i);
                    triangles.Add(BaseVertex + b.i);

                    // remove two nodes involved
                    RemoveNode(p);
                    RemoveNode(p.next);

                    p = start = b;
                }
                p = p.next;
            } while (p != start);

            return p;
        }

        // try splitting polygon into two and triangulate them independently
        private readonly void SplitEarcut(Node start, NativeList<int> triangles, float2 min, float invSize)
        {
            // look for a valid diagonal that divides the polygon into two
            var a = start;
            do
            {
                var b = a.next.next;
                while (b != a.prev)
                {
                    if (a.i != b.i && IsValidDiagonal(a, b))
                    {
                        // split the polygon in two by the diagonal
                        var c = SplitPolygon(a, b);

                        // filter colinear points around the cuts
                        a = FilterPoints(a, a.next);
                        c = FilterPoints(c, c.next);

                        // run earcut on each half
                        EarcutLinked(a, triangles, min, invSize);
                        EarcutLinked(c, triangles, min, invSize);
                        return;
                    }
                    b = b.next;
                }
                a = a.next;
            } while (a != start);
        }

        // interlink polygon nodes in z-order
        static void IndexCurve(Node start, float2 min, float invSize)
        {
            Node p = start;
            do
            {
                if (p.z == null)
                {
                    p.z = ZOrder(p.position, min, invSize);
                }

                p.prevZ = p.prev;
                p.nextZ = p.next;
                p = p.next;
            } while (p != start);

            p.prevZ.nextZ = null;
            p.prevZ = null;

            SortLinked(p);
        }

        // Simon Tatham's linked list merge sort algorithm
        // http://www.chiark.greenend.org.uk/~sgtatham/algorithms/listsort.html
        static Node SortLinked(Node list)
        {
            int i;
            Node p;
            Node q;
            Node e;
            Node tail;
            int numMerges;
            int pSize;
            int qSize;
            int inSize = 1;

            do
            {
                p = list;
                list = null;
                tail = null;
                numMerges = 0;

                while (p != null)
                {
                    numMerges++;
                    q = p;
                    pSize = 0;
                    for (i = 0; i < inSize; i++)
                    {
                        pSize++;
                        q = q.nextZ;
                        if (q == null)
                        {
                            break;
                        }
                    }
                    qSize = inSize;

                    while (pSize > 0 || (qSize > 0 && q != null))
                    {

                        if (pSize != 0 && (qSize == 0 || q == null || p.z <= q.z))
                        {
                            e = p;
                            p = p.nextZ;
                            pSize--;
                        }
                        else
                        {
                            e = q;
                            q = q.nextZ;
                            qSize--;
                        }

                        if (tail != null)
                        {
                            tail.nextZ = e;
                        }
                        else
                        {
                            list = e;
                        }

                        e.prevZ = tail;
                        tail = e;
                    }

                    p = q;
                }

                tail.nextZ = null;
                inSize *= 2;

            } while (numMerges > 1);

            return list;
        }

        // z-order of a point given coords and inverse of the longer side of data bbox
        static int ZOrder(float2 point, float2 min, float invSize)
        {
            // coords are transformed into non-negative 15-bit integer range
            int2 value = (int2)(32767 * (point - min) * invSize);

            value = (value | (value << 8)) & 0x00FF00FF;
            value = (value | (value << 4)) & 0x0F0F0F0F;
            value = (value | (value << 2)) & 0x33333333;
            value = (value | (value << 1)) & 0x55555555;

            return value.x | (value.y << 1);
        }

        // check if a point lies within a convex triangle
        static bool PointInTriangle(float ax, float ay, float bx, float by, float cx, float cy, float px, float py)
        {
            return (cx - px) * (ay - py) - (ax - px) * (cy - py) >= 0 &&
                   (ax - px) * (by - py) - (bx - px) * (ay - py) >= 0 &&
                   (bx - px) * (cy - py) - (cx - px) * (by - py) >= 0;
        }

        // check if a diagonal between two polygon nodes is valid (lies in polygon interior)
        static bool IsValidDiagonal(Node a, Node b)
        {
            return a.next.i != b.i && a.prev.i != b.i && !IntersectsPolygon(a, b) &&
                   LocallyInside(a, b) && LocallyInside(b, a) && MiddleInside(a, b);
        }

        // signed area of a triangle
        static float Area(Node p, Node q, Node r)
        {
            return (q.y - p.y) * (r.x - q.x) - (q.x - p.x) * (r.y - q.y);
        }

        // check if two points are equal
        static bool Equals(Node p1, Node p2)
        {
            return math.all(p1.position == p2.position);
        }

        // check if two segments intersect
        static bool Intersects(Node p1, Node q1, Node p2, Node q2)
        {
            if ((Equals(p1, q1) && Equals(p2, q2)) ||
                (Equals(p1, q2) && Equals(p2, q1)))
            {
                return true;
            }

            return Area(p1, q1, p2) > 0 != Area(p1, q1, q2) > 0 &&
                   Area(p2, q2, p1) > 0 != Area(p2, q2, q1) > 0;
        }

        // check if a polygon diagonal intersects any polygon segments
        static bool IntersectsPolygon(Node a, Node b)
        {
            Node p = a;
            do
            {
                if (p.i != a.i && p.next.i != a.i && p.i != b.i && p.next.i != b.i &&
                        Intersects(p, p.next, a, b))
                {
                    return true;
                }

                p = p.next;
            } while (p != a);

            return false;
        }

        // check if a polygon diagonal is locally inside the polygon
        static bool LocallyInside(Node a, Node b)
        {
            return Area(a.prev, a, a.next) < 0 ?
                Area(a, b, a.next) >= 0 && Area(a, a.prev, b) >= 0 :
                Area(a, b, a.prev) < 0 || Area(a, a.next, b) < 0;
        }

        // check if the middle point of a polygon diagonal is inside the polygon
        static bool MiddleInside(Node a, Node b)
        {
            var p = a;
            var inside = false;
            var position = (a.position + b.position) / 2;
            do
            {
                if (((p.y > position.y) != (p.next.y > position.y)) && p.next.y != p.y &&
                        (position.x < (p.next.x - p.x) * (position.y - p.y) / (p.next.y - p.y) + p.x))
                {
                    inside = !inside;
                }

                p = p.next;
            } while (p != a);

            return inside;
        }

        // link two polygon vertices with a bridge; if the vertices belong to the same ring, it splits polygon into two;
        // if one belongs to the outer ring and another to a hole, it merges it into a single ring
        static Node SplitPolygon(Node a, Node b)
        {
            var a2 = new Node(a.i, a.position);
            var b2 = new Node(b.i, b.position);
            var an = a.next;
            var bp = b.prev;

            a.next = b;
            b.prev = a;

            a2.next = an;
            an.prev = a2;

            b2.next = a2;
            a2.prev = b2;

            bp.next = b2;
            b2.prev = bp;

            return b2;
        }

        // create a node and optionally link it with previous one (in a circular doubly linked list)
        static Node InsertNode(int i, float2 position, Node last)
        {
            var p = new Node(i, position);

            if (last == null)
            {
                p.prev = p;
                p.next = p;

            }
            else
            {
                p.next = last.next;
                p.prev = last;
                last.next.prev = p;
                last.next = p;
            }
            return p;
        }

        static void RemoveNode(Node p)
        {
            p.next.prev = p.prev;
            p.prev.next = p.next;

            if (p.prevZ != null)
            {
                p.prevZ.nextZ = p.nextZ;
            }

            if (p.nextZ != null)
            {
                p.nextZ.prevZ = p.prevZ;
            }
        }

        public class Node
        {
            public int i;
            public float2 position;

            public float x => position.x;
            public float y => position.y;
            public int? z;

            public Node prev;
            public Node next;

            public Node prevZ;
            public Node nextZ;

            public Node(int i, float2 position)
            {
                // vertex index in coordinates array
                this.i = i;

                // vertex coordinates
                this.position = position;

                // previous and next vertex nodes in a polygon ring
                this.prev = null;
                this.next = null;

                // z-order curve value
                this.z = null;

                // previous and next nodes in z-order
                this.prevZ = null;
                this.nextZ = null;
            }
        }

        static float SignedArea(NativeSlice<float2> data, int start, int end)
        {
            var sum = default(float);

            for (int i = start, j = end - 1; i < end; i++)
            {
                sum += (data[j].x - data[i].x) * (data[i].y + data[j].y);
                j = i;
            }

            return sum;
        }
    }
}