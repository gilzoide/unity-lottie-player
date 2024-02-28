using System;
using Unity.Collections;
using UnityEngine;

namespace Gilzoide.LottiePlayer
{
    public class LottieAnimation : IDisposable
    {
        public NativeLottieAnimation NativeHandle { get; }

        public bool IsCreated => NativeHandle.IsCreated;

        public LottieAnimation(string path)
        {
            NativeHandle = new NativeLottieAnimation(path);
        }

        public LottieAnimation(string data, string key, string resourcePath)
        {
            NativeHandle = new NativeLottieAnimation(data, key, resourcePath);
        }

        ~LottieAnimation()
        {
            Dispose();
        }

        public void Dispose()
        {
            NativeHandle.Dispose();
        }

        public Vector2Int GetSize()
        {
            return NativeHandle.GetSize();
        }

        public double GetDuration()
        {
            return NativeHandle.GetDuration();
        }

        public int GetTotalFrame()
        {
            return NativeHandle.GetTotalFrame();
        }

        public double GetFrameRate()
        {
            return NativeHandle.GetFrameRate();
        }

        public int GetFrameAtPos(float pos)
        {
            return NativeHandle.GetFrameAtPos(pos);
        }

        public unsafe RLottie.LayerNode* RenderTree(uint frameNum, uint width, uint height)
        {
            return NativeHandle.RenderTree(frameNum, width, height);
        }

        public void Render(uint frameNum, uint width, uint height, NativeArray<Color32> buffer, uint? bytesPerLine = null)
        {
            NativeHandle.Render(frameNum, width, height, buffer, bytesPerLine);
        }

        public void Render(uint frameNum, uint width, uint height, Color32[] buffer, uint? bytesPerLine = null)
        {
            NativeHandle.Render(frameNum, width, height, buffer, bytesPerLine);
        }

#if UNITY_2021_2_OR_NEWER
        public void Render(uint frameNum, uint width, uint height,Span<Color32> buffer, uint? bytesPerLine = null)
        {
            NativeHandle.Render(frameNum, width, height, buffer, bytesPerLine);
        }
#endif

        public void Render(uint frameNum, Texture2D texture)
        {
            NativeHandle.Render(frameNum, texture);
        }

        public unsafe void Render(uint frameNum, uint width, uint height, Color32* buffer, uint bytesPerLine)
        {
            NativeHandle.Render(frameNum, width, height, buffer, bytesPerLine);
        }

        public void RenderAsync(uint frameNum, uint width, uint height, NativeArray<Color32> buffer, uint? bytesPerLine = null)
        {
            NativeHandle.RenderAsync(frameNum, width, height, buffer, bytesPerLine);
        }

        public unsafe void RenderAsync(uint frameNum, uint width, uint height, Color32* buffer, uint bytesPerLine)
        {
            NativeHandle.RenderAsync(frameNum, width, height, buffer, bytesPerLine);
        }

        public void RenderAsyncFlush()
        {
            NativeHandle.RenderAsyncFlush();
        }

        public unsafe RLottie.MarkerList* GetMarkerList()
        {
            return NativeHandle.GetMarkerList();
        }

        public void SetFillColorOverride(string keypath, Color value)
        {
            NativeHandle.SetFillColorOverride(keypath, value);
        }

        public void SetFillOpacityOverride(string keypath, float value)
        {
            NativeHandle.SetFillOpacityOverride(keypath, value);
        }

        public void SetStrokeColorOverride(string keypath, Color value)
        {
            NativeHandle.SetStrokeColorOverride(keypath, value);
        }

        public void SetStrokeOpacityOverride(string keypath, float value)
        {
            NativeHandle.SetStrokeOpacityOverride(keypath, value);
        }

        public void SetStrokeWidthOverride(string keypath, float value)
        {
            NativeHandle.SetStrokeWidthOverride(keypath, value);
        }

        // Not yet implemented in rlottie (for some reason)
        // public void SetTransformAnchorOverride(string keypath, Vector2 value)
        // {
        //     NativeHandle.SetTransformAnchorOverride(keypath, value);
        // }

        public void SetTransformPositionOverride(string keypath, Vector2 value)
        {
            NativeHandle.SetTransformPositionOverride(keypath, value);
        }

        public void SetTransformScaleOverride(string keypath, Vector2 value)
        {
            NativeHandle.SetTransformScaleOverride(keypath, value);
        }

        public void SetTransformRotationOverride(string keypath, float value)
        {
            NativeHandle.SetTransformRotationOverride(keypath, value);
        }

        // Not yet implemented in rlottie (for some reason)
        // public void SetTransformOpacityOverride(string keypath, float value)
        // {
        //     NativeHandle.SetTransformOpacityOverride(keypath, value);
        // }

        public static implicit operator NativeLottieAnimation(LottieAnimation animation)
        {
            return animation?.NativeHandle ?? default;
        }
    }
}
