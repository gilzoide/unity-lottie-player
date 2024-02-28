using System;
using Gilzoide.LottiePlayer.RLottie;
using UnityEngine;

namespace Gilzoide.LottiePlayer
{
    public class LottieAnimation : ILottieAnimation, IDisposable
    {
        public NativeLottieAnimation NativeHandle => _nativeHandle;
        private NativeLottieAnimation _nativeHandle;

        public bool IsCreated => _nativeHandle.IsCreated;

        public LottieAnimation(NativeLottieAnimation nativeHandle)
        {
            _nativeHandle = nativeHandle;
        }

        public LottieAnimation(string path)
        {
            _nativeHandle = new NativeLottieAnimation(path);
        }

        public LottieAnimation(string data, string key, string resourcePath)
        {
            _nativeHandle = new NativeLottieAnimation(data, key, resourcePath);
        }

        public unsafe LottieAnimation(byte* data, string key, string resourcePath)
        {
            _nativeHandle = new NativeLottieAnimation(data, key, resourcePath);
        }

        ~LottieAnimation()
        {
            Dispose();
        }

        public void Dispose()
        {
            _nativeHandle.Dispose();
        }

        public Vector2Int GetSize()
        {
            return _nativeHandle.GetSize();
        }

        public double GetDuration()
        {
            return _nativeHandle.GetDuration();
        }

        public int GetTotalFrame()
        {
            return _nativeHandle.GetTotalFrame();
        }

        public double GetFrameRate()
        {
            return _nativeHandle.GetFrameRate();
        }

        public int GetFrameAtPos(float pos)
        {
            return _nativeHandle.GetFrameAtPos(pos);
        }

        public unsafe LayerNode* RenderTree(uint frameNum, uint width, uint height)
        {
            return _nativeHandle.RenderTree(frameNum, width, height);
        }

        public unsafe void Render(uint frameNum, uint width, uint height, Color32* buffer, uint bytesPerLine)
        {
            _nativeHandle.Render(frameNum, width, height, buffer, bytesPerLine);
        }

        public unsafe void RenderAsync(uint frameNum, uint width, uint height, Color32* buffer, uint bytesPerLine)
        {
            _nativeHandle.RenderAsync(frameNum, width, height, buffer, bytesPerLine);
        }

        public void RenderAsyncFlush()
        {
            _nativeHandle.RenderAsyncFlush();
        }

        public unsafe MarkerList* GetMarkerList()
        {
            return _nativeHandle.GetMarkerList();
        }

        public void SetFillColorOverride(string keypath, Color value)
        {
            _nativeHandle.SetFillColorOverride(keypath, value);
        }

        public void SetFillOpacityOverride(string keypath, float value)
        {
            _nativeHandle.SetFillOpacityOverride(keypath, value);
        }

        public void SetStrokeColorOverride(string keypath, Color value)
        {
            _nativeHandle.SetStrokeColorOverride(keypath, value);
        }

        public void SetStrokeOpacityOverride(string keypath, float value)
        {
            _nativeHandle.SetStrokeOpacityOverride(keypath, value);
        }

        public void SetStrokeWidthOverride(string keypath, float value)
        {
            _nativeHandle.SetStrokeWidthOverride(keypath, value);
        }

        // Not yet implemented in rlottie (for some reason)
        // public void SetTransformAnchorOverride(string keypath, Vector2 value)
        // {
        //     _nativeHandle.SetTransformAnchorOverride(keypath, value);
        // }

        public void SetTransformPositionOverride(string keypath, Vector2 value)
        {
            _nativeHandle.SetTransformPositionOverride(keypath, value);
        }

        public void SetTransformScaleOverride(string keypath, Vector2 value)
        {
            _nativeHandle.SetTransformScaleOverride(keypath, value);
        }

        public void SetTransformRotationOverride(string keypath, float value)
        {
            _nativeHandle.SetTransformRotationOverride(keypath, value);
        }

        // Not yet implemented in rlottie (for some reason)
        // public void SetTransformOpacityOverride(string keypath, float value)
        // {
        //     _nativeHandle.SetTransformOpacityOverride(keypath, value);
        // }

        public static implicit operator NativeLottieAnimation(LottieAnimation animation)
        {
            return animation?._nativeHandle ?? default;
        }
    }
}
