using System;
using Gilzoide.LottiePlayer.RLottie;
using UnityEngine;

namespace Gilzoide.LottiePlayer
{
    public struct NativeLottieAnimation : ILottieAnimation, IDisposable
    {
        public IntPtr NativeHandle { get; private set; }

        public readonly bool IsCreated => NativeHandle != IntPtr.Zero;

        public NativeLottieAnimation(string path)
        {
            NativeHandle = RLottieCApi.lottie_animation_from_file(path);
        }

        public NativeLottieAnimation(string data, string key, string resourcePath)
        {
            NativeHandle = RLottieCApi.lottie_animation_from_data(data, key, resourcePath);
        }

        public unsafe NativeLottieAnimation(byte* data, string key, string resourcePath)
        {
            NativeHandle = RLottieCApi.lottie_animation_from_data(data, key, resourcePath);
        }

        public NativeLottieAnimation(IntPtr nativeHandle)
        {
            NativeHandle = nativeHandle;
        }

        public void Dispose()
        {
            if (IsCreated)
            {
                RLottieCApi.lottie_animation_destroy(NativeHandle);
                NativeHandle = IntPtr.Zero;
            }
        }

        public readonly Vector2Int GetSize()
        {
            RLottieCApi.lottie_animation_get_size(NativeHandle, out nuint width, out nuint height);
            return new Vector2Int(checked((int) width), checked((int) height));
        }

        public readonly double GetDuration()
        {
            return RLottieCApi.lottie_animation_get_duration(NativeHandle);
        }

        public readonly int GetTotalFrame()
        {
            return checked((int) RLottieCApi.lottie_animation_get_totalframe(NativeHandle));
        }

        public readonly double GetFrameRate()
        {
            return RLottieCApi.lottie_animation_get_framerate(NativeHandle);
        }

        public readonly int GetFrameAtPos(float pos)
        {
            return checked((int) RLottieCApi.lottie_animation_get_frame_at_pos(NativeHandle, pos));
        }

        public unsafe readonly LayerNode* RenderTree(uint frameNum, uint width, uint height)
        {
            return RLottieCApi.lottie_animation_render_tree(NativeHandle, frameNum, width, height);
        }

        public unsafe readonly void Render(uint frameNum, uint width, uint height, Color32* buffer, uint bytesPerLine)
        {
            RLottieCApi.lottie_animation_render(NativeHandle, frameNum, buffer, width, height, bytesPerLine);
        }

        public unsafe readonly void RenderAsync(uint frameNum, uint width, uint height, Color32* buffer, uint bytesPerLine)
        {
            RLottieCApi.lottie_animation_render_async(NativeHandle, frameNum, buffer, width, height, bytesPerLine);
        }

        public readonly void RenderAsyncFlush()
        {
            unsafe
            {
                RLottieCApi.lottie_animation_render_flush(NativeHandle);
            }
        }

        public readonly unsafe MarkerList* GetMarkerList()
        {
            return RLottieCApi.lottie_animation_get_markerlist(NativeHandle);
        }

        public readonly void SetFillColorOverride(string keypath, Color value)
        {
            SetPropertyOverride(AnimationProperty.FillColor, keypath, value);
        }

        public readonly void SetFillOpacityOverride(string keypath, float value)
        {
            SetPropertyOverride(AnimationProperty.FillOpacity, keypath, value);
        }

        public readonly void SetStrokeColorOverride(string keypath, Color value)
        {
            SetPropertyOverride(AnimationProperty.StrokeColor, keypath, value);
        }

        public readonly void SetStrokeOpacityOverride(string keypath, float value)
        {
            SetPropertyOverride(AnimationProperty.StrokeOpacity, keypath, value);
        }

        public readonly void SetStrokeWidthOverride(string keypath, float value)
        {
            SetPropertyOverride(AnimationProperty.StrokeWidth, keypath, value);
        }

        // Not yet implemented in rlottie (for some reason)
        // public readonly void SetTransformAnchorOverride(string keypath, Vector2 value)
        // {
        //     SetPropertyOverride(AnimationProperty.TransformAnchor, keypath, value);
        // }

        public readonly void SetTransformPositionOverride(string keypath, Vector2 value)
        {
            SetPropertyOverride(AnimationProperty.TransformPosition, keypath, value);
        }

        public readonly void SetTransformScaleOverride(string keypath, Vector2 value)
        {
            SetPropertyOverride(AnimationProperty.TransformScale, keypath, value);
        }

        public readonly void SetTransformRotationOverride(string keypath, float value)
        {
            SetPropertyOverride(AnimationProperty.TransformRotation, keypath, value);
        }

        // Not yet implemented in rlottie (for some reason)
        // public readonly void SetTransformOpacityOverride(string keypath, float value)
        // {
        //     SetPropertyOverride(AnimationProperty.TransformOpacity, keypath, value);
        // }

        private readonly void SetPropertyOverride(AnimationProperty type, string keypath, float value)
        {
            RLottieCApi.lottie_animation_property_override(NativeHandle, type, keypath, value);
        }
        private readonly void SetPropertyOverride(AnimationProperty type, string keypath, Vector2 value)
        {
            RLottieCApi.lottie_animation_property_override(NativeHandle, type, keypath, value.x, value.y);
        }
        private readonly void SetPropertyOverride(AnimationProperty type, string keypath, Color value)
        {
            RLottieCApi.lottie_animation_property_override(NativeHandle, type, keypath, value.r, value.g, value.b);
        }
    }
}
