using System;
using Gilzoide.LottiePlayer.RLottie;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Gilzoide.LottiePlayer
{
    public struct NativeLottieAnimation : ILottieAnimation, IDisposable
    {
        [field: NativeDisableUnsafePtrRestriction]
        public IntPtr NativeHandle { get; private set; }

        public readonly bool IsCreated => NativeHandle != IntPtr.Zero;
        public static NativeLottieAnimation Invalid => new();

        public NativeLottieAnimation(string path)
        {
            NativeHandle = RLottieCApi.lottie_animation_from_file(path);
        }

        public NativeLottieAnimation(string data, string key, string resourcePath)
        {
            NativeHandle = RLottieCApi.lottie_animation_from_data(data, key, resourcePath);
        }

        public NativeLottieAnimation(IntPtr nativeHandle)
        {
            NativeHandle = nativeHandle;
        }

        public override readonly bool Equals(object obj)
        {
            return obj is NativeLottieAnimation other
                && other.NativeHandle == NativeHandle;
        }

        public override readonly int GetHashCode()
        {
            return NativeHandle.GetHashCode();
        }

        public static bool operator ==(NativeLottieAnimation a, NativeLottieAnimation b)
        {
            return a.NativeHandle == b.NativeHandle;
        }

        public static bool operator !=(NativeLottieAnimation a, NativeLottieAnimation b)
        {
            return a.NativeHandle != b.NativeHandle;
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
            ThrowIfNotCreated();
            RLottieCApi.lottie_animation_get_size(NativeHandle, out nuint width, out nuint height);
            return new Vector2Int(checked((int) width), checked((int) height));
        }

        public readonly double GetDuration()
        {
            ThrowIfNotCreated();
            return RLottieCApi.lottie_animation_get_duration(NativeHandle);
        }

        public readonly uint GetTotalFrame()
        {
            ThrowIfNotCreated();
            return checked((uint) RLottieCApi.lottie_animation_get_totalframe(NativeHandle));
        }

        public readonly double GetFrameRate()
        {
            ThrowIfNotCreated();
            return RLottieCApi.lottie_animation_get_framerate(NativeHandle);
        }

        public readonly uint GetFrameAtPos(float pos)
        {
            ThrowIfNotCreated();
            return checked((uint) RLottieCApi.lottie_animation_get_frame_at_pos(NativeHandle, pos));
        }

        public unsafe readonly LayerNode* RenderTree(uint frameNum, uint width, uint height)
        {
            ThrowIfNotCreated();
            return RLottieCApi.lottie_animation_render_tree(NativeHandle, frameNum, width, height);
        }

        public unsafe readonly void Render(uint frameNum, uint width, uint height, Color32* buffer, bool keepAspectRatio = true)
        {
            ThrowIfNotCreated();
            RLottieCApi.lottie_animation_render_aspect(NativeHandle, frameNum, buffer, width, height, width * (uint) UnsafeUtility.SizeOf<Color32>(), keepAspectRatio ? 1 : 0);
        }

        public unsafe readonly void RenderAsync(uint frameNum, uint width, uint height, Color32* buffer, bool keepAspectRatio = true)
        {
            ThrowIfNotCreated();
            RLottieCApi.lottie_animation_render_async_aspect(NativeHandle, frameNum, buffer, width, height, width * (uint) UnsafeUtility.SizeOf<Color32>(), keepAspectRatio ? 1 : 0);
        }

        public readonly void RenderAsyncFlush()
        {
            ThrowIfNotCreated();
            unsafe
            {
                RLottieCApi.lottie_animation_render_flush(NativeHandle);
            }
        }

        public readonly unsafe MarkerList* GetMarkerList()
        {
            ThrowIfNotCreated();
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
            ThrowIfNotCreated();
            RLottieCApi.lottie_animation_property_override(NativeHandle, type, keypath, value);
        }
        private readonly void SetPropertyOverride(AnimationProperty type, string keypath, Vector2 value)
        {
            ThrowIfNotCreated();
            RLottieCApi.lottie_animation_property_override(NativeHandle, type, keypath, value.x, value.y);
        }
        private readonly void SetPropertyOverride(AnimationProperty type, string keypath, Color value)
        {
            ThrowIfNotCreated();
            RLottieCApi.lottie_animation_property_override(NativeHandle, type, keypath, value.r, value.g, value.b);
        }

        private readonly void ThrowIfNotCreated()
        {
            if (!IsCreated)
            {
                throw new NullReferenceException("Animation is null");
            }
        }
    }
}
