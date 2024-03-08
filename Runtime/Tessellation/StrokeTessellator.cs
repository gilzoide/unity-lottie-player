using Gilzoide.LottiePlayer.RLottie;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Gilzoide.LottiePlayer.Tessellation
{
    [BurstCompile]
    public struct StrokeTessellatorJob : IJob
    {
        [NativeDisableUnsafePtrRestriction] public Path Path;
        public NativeList<Vector3> OutVertices;
        public NativeList<int> OutIndices;
        public Node.Stroke Stroke;
        public float Step;
        public float Z;

        public void Execute()
        {
            float halfWidth = Stroke.width * 0.5f;
            int lastIndex = OutVertices.Length;
            using (var pathEnumerator = new PathEnumerator(Path, Step))
            using (var skipEnumerator = new SkipRepeatedEnumerator<Vector2, PathEnumerator>(pathEnumerator))
            using (var enumerator = new PairOfPairsEnumerator<Vector2, SkipRepeatedEnumerator<Vector2, PathEnumerator>>(skipEnumerator))
            while (enumerator.MoveNext())
            {
                switch (enumerator.Current)
                {
                    case (null, Vector2 first, Vector2 next):
                    {
                        float2 v = next - first;
                        float2 normal = math.normalize(new float2(-v.y, v.x)) * halfWidth;
                        OutVertices.Add(new float3((float2) first + normal, Z));
                        OutVertices.Add(new float3((float2) first - normal, Z));
                        lastIndex += 2;
                        break;
                    }

                    case (Vector2 previous, Vector2 current, Vector2 next):
                    {
                        float2 vCurrent = current - previous;
                        float2 vNext = next - current;
                        float2 normal = math.normalize(new float2(-vCurrent.y, vCurrent.x)) * halfWidth;
                        OutVertices.Add(new float3((float2) current + normal, Z));
                        OutVertices.Add(new float3((float2) current - normal, Z));
                        OutIndices.Add(lastIndex - 2);
                        OutIndices.Add(lastIndex - 1);
                        OutIndices.Add(lastIndex);
                        OutIndices.Add(lastIndex - 1);
                        OutIndices.Add(lastIndex);
                        OutIndices.Add(lastIndex + 1);
                        lastIndex += 2;
                        break;
                    }

                    case (Vector2 previous, Vector2 last, null):
                    {
                        float2 v = last - previous;
                        float2 normal = math.normalize(new float2(-v.y, v.x)) * halfWidth;
                        OutVertices.Add(new float3((float2) last + normal, Z));
                        OutVertices.Add(new float3((float2) last - normal, Z));
                        OutIndices.Add(lastIndex - 2);
                        OutIndices.Add(lastIndex - 1);
                        OutIndices.Add(lastIndex);
                        OutIndices.Add(lastIndex - 1);
                        OutIndices.Add(lastIndex);
                        OutIndices.Add(lastIndex + 1);
                        // lastIndex += 2;  // this is the last iteration, so no need to update lastIndex
                        break;
                    }
                }
            }
        }
    }
}
