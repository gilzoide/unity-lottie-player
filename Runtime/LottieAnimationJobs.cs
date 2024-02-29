using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

namespace Gilzoide.LottiePlayer
{
    public static class LottieAnimationJobs
    {
        public static LottieAnimationRenderJob CreateRenderJob(this NativeLottieAnimation animation, uint frameNum, Texture2D texture)
        {
            return new LottieAnimationRenderJob(animation, frameNum, texture);
        }

        public static LottieAnimationRenderJob CreateRenderJob(this NativeLottieAnimation animation, uint frameNum, uint width, uint height, NativeArray<Color32> buffer, uint? bytesPerLine = null)
        {
            return new LottieAnimationRenderJob(animation, frameNum, width, height, buffer, bytesPerLine);
        }
    }

    [BurstCompile]
    public struct LottieAnimationRenderJob : IJob
    {
        private NativeLottieAnimation Animation;
        private uint Frame;
        private uint Width;
        private uint Height;
        private NativeArray<Color32> Buffer;
        private uint? BytesPerLine;

        public LottieAnimationRenderJob(NativeLottieAnimation animation, uint frameNum, Texture2D texture)
            : this(animation, frameNum, (uint) texture.width, (uint) texture.height, texture.GetRawTextureData<Color32>(), (uint) texture.width * (uint) UnsafeUtility.SizeOf<Color32>())
        {
        }

        public LottieAnimationRenderJob(NativeLottieAnimation animation, uint frameNum, uint width, uint height, NativeArray<Color32> buffer, uint? bytesPerLine = null)
        {
            Animation = animation;
            Frame = frameNum;
            Width = width;
            Height = height;
            Buffer = buffer;
            BytesPerLine = bytesPerLine;
        }

        public readonly void Execute()
        {
            Animation.Render(Frame, Width, Height, Buffer, BytesPerLine);
        }
    }
}
