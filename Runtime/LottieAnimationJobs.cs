using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Gilzoide.LottiePlayer
{
    public static class LottieAnimationJobs
    {
        public static LottieAnimationRenderJob CreateRenderJob(this NativeLottieAnimation animation, uint frameNum, Texture2D texture, bool keepAspectRatio = true)
        {
            return new LottieAnimationRenderJob(animation, frameNum, texture, keepAspectRatio);
        }

        public static LottieAnimationRenderJob CreateRenderJob(this NativeLottieAnimation animation, uint frameNum, uint width, uint height, NativeArray<Color32> buffer, bool keepAspectRatio = true)
        {
            return new LottieAnimationRenderJob(animation, frameNum, width, height, buffer, keepAspectRatio);
        }

        [Obsolete("bytesPerLine is ignored, prefer the overload that does not receive it.")]
        public static LottieAnimationRenderJob CreateRenderJob(this NativeLottieAnimation animation, uint frameNum, uint width, uint height, NativeArray<Color32> buffer, uint? bytesPerLine, bool keepAspectRatio = true)
        {
            return CreateRenderJob(animation, frameNum, width, height, buffer, keepAspectRatio);
        }
    }

    [BurstCompile]
    public struct LottieAnimationRenderJob : IJob
    {
        private NativeLottieAnimation Animation;
        private readonly uint Frame;
        private readonly uint Width;
        private readonly uint Height;
        private NativeArray<Color32> Buffer;
        private readonly bool KeepAspectRatio;

        public LottieAnimationRenderJob(NativeLottieAnimation animation, uint frameNum, Texture2D texture, bool keepAspectRatio = true)
            : this(animation, frameNum, (uint) texture.width, (uint) texture.height, texture.GetRawTextureData<Color32>(), keepAspectRatio)
        {
        }

        public LottieAnimationRenderJob(NativeLottieAnimation animation, uint frameNum, uint width, uint height, NativeArray<Color32> buffer, bool keepAspectRatio = true)
        {
            Animation = animation;
            Frame = frameNum;
            Width = width;
            Height = height;
            Buffer = buffer;
            KeepAspectRatio = keepAspectRatio;
        }

        [Obsolete("bytesPerLine is ignored, prefer the overload that does not receive it.")]
        public LottieAnimationRenderJob(NativeLottieAnimation animation, uint frameNum, uint width, uint height, NativeArray<Color32> buffer, uint? bytesPerLine, bool keepAspectRatio = true)
            : this(animation, frameNum, width, height, buffer, keepAspectRatio)
        {
        }

        public readonly void Execute()
        {
            Animation.Render(Frame, Width, Height, Buffer, KeepAspectRatio);
        }
    }
}
