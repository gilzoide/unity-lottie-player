using System;
using Gilzoide.LottiePlayer.RLottie;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Gilzoide.LottiePlayer
{
    public interface ILottieAnimation
    {
        bool IsCreated { get; }

        Vector2Int GetSize();
        double GetDuration();
        uint GetTotalFrame();
        double GetFrameRate();
        uint GetFrameAtPos(float pos);

        unsafe LayerNode* RenderTree(uint frameNum, uint width, uint height);
        unsafe void Render(uint frameNum, uint width, uint height, Color32* buffer, uint bytesPerLine);
        unsafe void RenderAsync(uint frameNum, uint width, uint height, Color32* buffer, uint bytesPerLine);
        void RenderAsyncFlush();

        unsafe MarkerList* GetMarkerList();

        void SetFillColorOverride(string keypath, Color value);
        void SetFillOpacityOverride(string keypath, float value);
        void SetStrokeColorOverride(string keypath, Color value);
        void SetStrokeOpacityOverride(string keypath, float value);
        void SetStrokeWidthOverride(string keypath, float value);
        // Not yet implemented in rlottie (for some reason)
        // void SetTransformAnchorOverride(string keypath, Vector2 value);
        void SetTransformPositionOverride(string keypath, Vector2 value);
        void SetTransformScaleOverride(string keypath, Vector2 value);
        void SetTransformRotationOverride(string keypath, float value);
        // Not yet implemented in rlottie (for some reason)
        // void SetTransformOpacityOverride(string keypath, float value);
    }

    public static class ILottieAnimationExtensions
    {
        public static bool IsValid<TAnimation>(this TAnimation animation)
            where TAnimation : ILottieAnimation
        {
            return animation?.IsCreated ?? false;
        }

        public static uint GetFrameAtTime<TAnimation>(this TAnimation animation, float time, bool loop = true)
            where TAnimation : ILottieAnimation
        {
            float duration = (float) animation.GetDuration();
            if (duration <= 0)
            {
                return 0;
            }
            if (loop)
            {
                time = Mathf.Repeat(time, duration);
            }
            return animation.GetFrameAtPos(time / duration);
        }

        public static Texture2D CreateTexture<TAnimation>(this TAnimation animation, bool mipChain = false, bool linear = false)
            where TAnimation : ILottieAnimation
        {
            if (animation.IsValid())
            {
                Vector2Int size = animation.GetSize();
                return CreateTexture(animation, size.x, size.y, mipChain, linear);
            }
            else
            {
                return null;
            }
        }

        public static Texture2D CreateTexture<TAnimation>(this TAnimation animation, int width, int height, bool mipChain = false, bool linear = false)
            where TAnimation : ILottieAnimation
        {
            return animation.IsValid() ? new Texture2D(width, height, TextureFormat.BGRA32, mipChain, linear) : null;
        }

        public static void Render<TAnimation>(this TAnimation animation, uint frameNum, uint width, uint height, NativeArray<Color32> buffer, uint? bytesPerLine = null)
            where TAnimation : ILottieAnimation
        {
            unsafe
            {
                animation.Render(frameNum, width, height, (Color32*) buffer.GetUnsafePtr(), bytesPerLine ?? (uint) buffer.Length / width);
            }
        }

        public static void Render<TAnimation>(this TAnimation animation, uint frameNum, uint width, uint height, Color32[] buffer, uint? bytesPerLine = null)
            where TAnimation : ILottieAnimation
        {
            unsafe
            {
                fixed (Color32* ptr = buffer)
                {
                    animation.Render(frameNum, width, height, ptr, bytesPerLine ?? (uint) buffer.Length / width);
                }
            }
        }

#if UNITY_2021_2_OR_NEWER
        public static void Render<TAnimation>(this TAnimation animation, uint frameNum, uint width, uint height, ReadOnlySpan<Color32> buffer, uint? bytesPerLine = null)
            where TAnimation : ILottieAnimation
        {
            unsafe
            {
                fixed (Color32* ptr = buffer)
                {
                    animation.Render(frameNum, width, height, ptr, bytesPerLine ?? (uint) buffer.Length / width);
                }
            }
        }
#endif

        public static void Render<TAnimation>(this TAnimation animation, uint frameNum, Texture2D texture)
            where TAnimation : ILottieAnimation
        {
            if (texture == null)
            {
                throw new ArgumentNullException(nameof(texture));
            }
            animation.Render(frameNum, (uint) texture.width, (uint) texture.height, texture.GetRawTextureData<Color32>(), (uint) texture.width * (uint) UnsafeUtility.SizeOf<Color32>());
        }

        public static void RenderAsync<TAnimation>(this TAnimation animation, uint frameNum, uint width, uint height, NativeArray<Color32> buffer, uint? bytesPerLine = null)
            where TAnimation : ILottieAnimation
        {
            unsafe
            {
                animation.RenderAsync(frameNum, width, height, (Color32*) buffer.GetUnsafePtr(), bytesPerLine ?? (uint) buffer.Length / width);
            }
        }
    }
}
