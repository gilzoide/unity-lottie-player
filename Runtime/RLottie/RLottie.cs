using System;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;


namespace Gilzoide.LottiePlayer.RLottie
{
    using Lottie_Animation = IntPtr;

    // rlottie_common.h
    public enum BrushType
    {
        Solid = 0,
        Gradient,
    }

    public enum FillRule
    {
        EvenOdd = 0,
        Winding,
    }

    public enum JoinStyle
    {
        Miter = 0,
        Bevel,
        Round,
    }

    public enum CapStyle
    {
        Flat = 0,
        Square,
        Round,
    }

    public enum GradientType
    {
        Linear = 0,
        Radial,
    }

    public struct GradientStop
    {
        public float pos;
        // unsigned char r, g, b, a;
        public Color32 color;
    }

    public enum MaskType
    {
        Add = 0,
        Substract,
        Intersect,
        Difference,
    }

    public unsafe struct Path
    {
        public Vector2* ptPtr;
        public UIntPtr ptCount;
        public Element* elmPtr;
        public UIntPtr elmCount;

        public enum Element : byte
        {
            MoveTo,
            LineTo,
            CubicTo,
            Close,
        }
    }

    public struct Mask
    {
        // struct {
        //     const float *ptPtr;
        //     UIntPtr       ptCount;
        //     const char  *elmPtr;
        //     UIntPtr       elmCount;
        // }
        public Path mPath;
        public MaskType mMode;
        public byte mAlpha;
    }

    public enum MatteType
    {
        None = 0,
        Alpha,
        AlphaInv,
        Luma,
        LumaInv,
    }

    public unsafe struct Marker
    {
        public char* name;
        public UIntPtr startframe;
        public UIntPtr endframe;
    }

    public unsafe struct MarkerList
    {
        public Marker* ptr;
        public UIntPtr size;
    }

    public enum ChangeFlag : int
    {
        None = 0x0000,
        Path = 0x0001,
        Paint = 0x0010,
        All = Path | Paint,
    }

    public unsafe struct Node
    {
        // struct {
        //     const float *ptPtr;
        //     UIntPtr       ptCount;
        //     const char  *elmPtr;
        //     UIntPtr       elmCount;
        // }
        public Path mPath;

        // struct {
        //     unsigned char r, g, b, a;
        // }
        public Color32 mColor;

        public struct Stroke
        {
            public byte enable;
            public float width;
            public CapStyle cap;
            public JoinStyle join;
            public float miterLimit;
            public float* dashArray;
            public int dashArraySize;
        }
        public Stroke mStroke;

        public struct Gradient
        {
            public GradientType  type;
            public GradientStop* stopPtr;
            public UIntPtr stopCount;
            // struct {
            //     float x, y;
            // }
            public Vector2 start, end, center, focal;
            public float cradius;
            public float fradius;
        }
        public Gradient mGradient;

        public struct ImageInfo
        {
            public byte* data;
            public UIntPtr width;
            public UIntPtr height;
            public byte mAlpha;
            // struct {
            //     float m11; float m12; float m13;
            //     float m21; float m22; float m23;
            //     float m31; float m32; float m33;
            // }
            public float3x3 mMatrix;
        }
        public ImageInfo mImageInfo;

        public ChangeFlag mFlag;
        public BrushType mBrushType;
        public FillRule  mFillRule;

        public char* keypath;
    }

    public unsafe struct LayerNode
    {

        // struct {
        //     LOTMask *ptr;
        //     UIntPtr size;
        // }
        public Mask* mMaskList_ptr;
        public UIntPtr mMaskList_size;

        // struct {
        //     const float *ptPtr;
        //     UIntPtr ptCount;
        //     const char *elmPtr;
        //     UIntPtr elmCount;
        // }
        public Path mClipPath;

        // struct {
        //     struct LOTLayerNode **ptr;
        //     UIntPtr size;
        // }
        public LayerNode** mLayerList_ptr;
        public UIntPtr mLayerList_size;

        // struct {
        //     LOTNode **ptr;
        //     UIntPtr size;
        // }
        public Node** mNodeList_ptr;
        public UIntPtr mNodeList_size;

        public MatteType mMatte;
        public int mVisible;
        public byte mAlpha;
        public char* keypath;
    }

    // rlottie_capi.h
    public enum AnimationProperty
    {
        FillColor,      /*!< Color property of Fill object , value type is float [0 ... 1] */
        FillOpacity,    /*!< Opacity property of Fill object , value type is float [ 0 .. 100] */
        StrokeColor,    /*!< Color property of Stroke object , value type is float [0 ... 1] */
        StrokeOpacity,  /*!< Opacity property of Stroke object , value type is float [ 0 .. 100] */
        StrokeWidth,    /*!< stroke with property of Stroke object , value type is float */
        TransformAnchor,      /*!< Transform Anchor property of Layer and Group object , value type is int */
        TransformPosition,    /*!< Transform Position property of Layer and Group object , value type is int */
        TransformScale,       /*!< Transform Scale property of Layer and Group object , value type is float range[0 ..100] */
        TransformRotation,    /*!< Transform Scale property of Layer and Group object , value type is float. range[0 .. 360] in degrees*/
        TransformOpacity,     /*!< Transform Opacity property of Layer and Group object , value type is float [ 0 .. 100] */
    }

    public static unsafe class RLottieCApi
    {
#if !UNITY_EDITOR && (UNITY_WEBGL || UNITY_IOS || UNITY_TVOS || UNITY_VISIONOS)
        internal const string LibraryName = "__Internal";
#else
        internal const string LibraryName = "lottie-player";
#endif

        [DllImport(LibraryName, CharSet = CharSet.Ansi)]
        public static extern Lottie_Animation lottie_animation_from_file(string path);

        [DllImport(LibraryName, CharSet = CharSet.Ansi)]
        public static extern Lottie_Animation lottie_animation_from_data(string data, string key, string resourcePath);

        [DllImport(LibraryName)]
        public static extern void lottie_animation_destroy(Lottie_Animation animation);

        [DllImport(LibraryName)]
        public static extern void lottie_animation_get_size(Lottie_Animation animation, out UIntPtr width, out UIntPtr height);

        [DllImport(LibraryName)]
        public static extern double lottie_animation_get_duration(Lottie_Animation animation);

        [DllImport(LibraryName)]
        public static extern UIntPtr lottie_animation_get_totalframe(Lottie_Animation animation);

        [DllImport(LibraryName)]
        public static extern double lottie_animation_get_framerate(Lottie_Animation animation);

        [DllImport(LibraryName)]
        public static extern LayerNode* lottie_animation_render_tree(Lottie_Animation animation, UIntPtr frame_num, UIntPtr width, UIntPtr height);

        [DllImport(LibraryName)]
        public static extern UIntPtr lottie_animation_get_frame_at_pos(Lottie_Animation animation, float pos);

        [DllImport(LibraryName)]
        public static extern void lottie_animation_render_aspect(Lottie_Animation animation, UIntPtr frame_num, Color32* buffer, UIntPtr width, UIntPtr height, UIntPtr bytes_per_line, int keepAspect);

        [DllImport(LibraryName)]
        public static extern void lottie_animation_render_async_aspect(Lottie_Animation animation, UIntPtr frame_num, Color32* buffer, UIntPtr width, UIntPtr height, UIntPtr bytes_per_line, int keepAspect);

        [DllImport(LibraryName)]
        public static extern Color32* lottie_animation_render_flush(Lottie_Animation animation);

        [DllImport(LibraryName, CharSet = CharSet.Ansi)]
        public static extern void lottie_animation_property_override(Lottie_Animation animation, AnimationProperty type, string keypath, double value);

        [DllImport(LibraryName, CharSet = CharSet.Ansi)]
        public static extern void lottie_animation_property_override(Lottie_Animation animation, AnimationProperty type, string keypath, double value1, double value2);

        [DllImport(LibraryName, CharSet = CharSet.Ansi)]
        public static extern void lottie_animation_property_override(Lottie_Animation animation, AnimationProperty type, string keypath, double value1, double value2, double value3);

        [DllImport(LibraryName)]
        public static extern MarkerList* lottie_animation_get_markerlist(Lottie_Animation animation);

        [DllImport(LibraryName)]
        public static extern void lottie_configure_model_cache_size(UIntPtr cacheSize);
    }
}
