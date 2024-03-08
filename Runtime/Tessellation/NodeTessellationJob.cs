using Gilzoide.LottiePlayer.RLottie;
using Gilzoide.LottiePlayer.Tessellation.EarcutNet;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

namespace Gilzoide.LottiePlayer.Tessellation
{
    [BurstCompile]
    public unsafe struct NodeTessellationJob : IJob
    {
        [NativeDisableUnsafePtrRestriction] public Node* Node;
        public NativeList<Vector3> OutVertices;
        public NativeList<Color32> OutColors;
        public NativeList<int> OutIndices;
        [NativeDisableUnsafePtrRestriction] public AllocatorHelper<RewindableAllocator> NodeAllocator;
        public float Step;
        public float Z;

        public void Execute()
        {
            int baseVertex = OutVertices.Length;
            if (Node->mStroke.enable == 0)
            {
                NativeArray<Vector3> data = Node->mBrushType == BrushType.Solid ? FillPathSolid() : FillPathGradient();
                new EarcutJob
                {
                    Data = data,
                    OutIndices = OutIndices,
                    NodeAllocator = NodeAllocator,
                    BaseVertex = baseVertex,
                }.Execute();
            }
            else
            {
                new StrokeTessellatorJob
                {
                    Path = Node->mPath,
                    Stroke = Node->mStroke,
                    Step = Step,
                    OutVertices = OutVertices,
                    OutIndices = OutIndices,
                    Z = Z,
                }.Execute();

                Color32 color = Node->mColor;
                for (int i = baseVertex, count = OutVertices.Length; i < count; i++)
                {
                    OutColors.Add(color);
                }
            }
        }

        private NativeArray<Vector3> FillPathSolid()
        {
            int baseVertex = OutVertices.Length;
            Color32 color = Node->mColor;
            foreach (Vector2 point in new PathEnumerable(Node->mPath, Step))
            {
                OutVertices.Add(point.WithZ(Z));
                OutColors.Add(color);
            }
            return OutVertices.AsArray().GetSubArray(baseVertex, OutVertices.Length - baseVertex);
        }

        private NativeArray<Vector3> FillPathGradient()
        {
            int baseVertex = OutVertices.Length;
            Node.Gradient gradient = Node->mGradient;
            foreach (Vector2 point in new PathEnumerable(Node->mPath, Step))
            {
                OutVertices.Add(point.WithZ(Z));
                OutColors.Add(gradient.ColorForPoint(point));
            }
            return OutVertices.AsArray().GetSubArray(baseVertex, OutVertices.Length - baseVertex);
        }
    }
}
