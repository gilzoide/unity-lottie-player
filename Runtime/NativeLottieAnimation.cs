using System;
using UnityEngine;

namespace Gilzoide.LottiePlayer
{
    public struct NativeLottieAnimation : ILottieAnimation, IDisposable
    {
        public IntPtr NativeHandle { get; private set; }

        public readonly bool IsCreated => NativeHandle != IntPtr.Zero;

        public NativeLottieAnimation(string path)
        {
            NativeHandle = RLottie.lottie_animation_from_file(path);
        }

        public NativeLottieAnimation(string data, string key, string resourcePath)
        {
            NativeHandle = RLottie.lottie_animation_from_data(data, key, resourcePath);
        }

        public NativeLottieAnimation(IntPtr nativeHandle)
        {
            NativeHandle = nativeHandle;
        }

        public void Dispose()
        {
            if (IsCreated)
            {
                RLottie.lottie_animation_destroy(NativeHandle);
                NativeHandle = IntPtr.Zero;
            }
        }

        public readonly Vector2Int GetSize()
        {
            RLottie.lottie_animation_get_size(NativeHandle, out nuint width, out nuint height);
            return new Vector2Int(checked((int) width), checked((int) height));
        }

        public readonly double GetDuration()
        {
            return RLottie.lottie_animation_get_duration(NativeHandle);
        }

        public readonly int GetTotalFrame()
        {
            return checked((int) RLottie.lottie_animation_get_totalframe(NativeHandle));
        }

        public readonly double GetFrameRate()
        {
            return RLottie.lottie_animation_get_framerate(NativeHandle);
        }

        public readonly int GetFrameAtPos(float pos)
        {
            return checked((int) RLottie.lottie_animation_get_frame_at_pos(NativeHandle, pos));
        }

        public unsafe readonly RLottie.LayerNode* RenderTree(uint frameNum, uint width, uint height)
        {
            return RLottie.lottie_animation_render_tree(NativeHandle, frameNum, width, height);
        }

        public unsafe readonly void Render(uint frameNum, uint width, uint height, Color32* buffer, uint bytesPerLine)
        {
            RLottie.lottie_animation_render(NativeHandle, frameNum, buffer, width, height, bytesPerLine);
        }

        public unsafe readonly void RenderAsync(uint frameNum, uint width, uint height, Color32* buffer, uint bytesPerLine)
        {
            RLottie.lottie_animation_render_async(NativeHandle, frameNum, buffer, width, height, bytesPerLine);
        }

        public readonly void RenderAsyncFlush()
        {
            unsafe
            {
                RLottie.lottie_animation_render_flush(NativeHandle);
            }
        }

        public readonly unsafe RLottie.MarkerList* GetMarkerList()
        {
            return RLottie.lottie_animation_get_markerlist(NativeHandle);
        }

        public readonly void SetFillColorOverride(string keypath, Color value)
        {
            SetPropertyOverride(RLottie.AnimationProperty.FillColor, keypath, value);
        }

        public readonly void SetFillOpacityOverride(string keypath, float value)
        {
            SetPropertyOverride(RLottie.AnimationProperty.FillOpacity, keypath, value);
        }

        public readonly void SetStrokeColorOverride(string keypath, Color value)
        {
            SetPropertyOverride(RLottie.AnimationProperty.StrokeColor, keypath, value);
        }

        public readonly void SetStrokeOpacityOverride(string keypath, float value)
        {
            SetPropertyOverride(RLottie.AnimationProperty.StrokeOpacity, keypath, value);
        }

        public readonly void SetStrokeWidthOverride(string keypath, float value)
        {
            SetPropertyOverride(RLottie.AnimationProperty.StrokeWidth, keypath, value);
        }

        // Not yet implemented in rlottie (for some reason)
        // public readonly void SetTransformAnchorOverride(string keypath, Vector2 value)
        // {
        //     SetPropertyOverride(RLottie.AnimationProperty.TransformAnchor, keypath, value);
        // }

        public readonly void SetTransformPositionOverride(string keypath, Vector2 value)
        {
            SetPropertyOverride(RLottie.AnimationProperty.TransformPosition, keypath, value);
        }

        public readonly void SetTransformScaleOverride(string keypath, Vector2 value)
        {
            SetPropertyOverride(RLottie.AnimationProperty.TransformScale, keypath, value);
        }

        public readonly void SetTransformRotationOverride(string keypath, float value)
        {
            SetPropertyOverride(RLottie.AnimationProperty.TransformRotation, keypath, value);
        }

        // Not yet implemented in rlottie (for some reason)
        // public readonly void SetTransformOpacityOverride(string keypath, float value)
        // {
        //     SetPropertyOverride(RLottie.AnimationProperty.TransformOpacity, keypath, value);
        // }

        private readonly void SetPropertyOverride(RLottie.AnimationProperty type, string keypath, float value)
        {
            RLottie.lottie_animation_property_override(NativeHandle, type, keypath, value);
        }
        private readonly void SetPropertyOverride(RLottie.AnimationProperty type, string keypath, Vector2 value)
        {
            RLottie.lottie_animation_property_override(NativeHandle, type, keypath, value.x, value.y);
        }
        private readonly void SetPropertyOverride(RLottie.AnimationProperty type, string keypath, Color value)
        {
            RLottie.lottie_animation_property_override(NativeHandle, type, keypath, value.r, value.g, value.b);
        }
    }
}
