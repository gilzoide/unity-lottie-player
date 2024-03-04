// amalgamated rlottie build, to greatly simplify build process
    #define RLOTTIE_BUILD
    #include "binding/c/lottieanimation_capi.cpp"
    #include "lottie/lottieanimation.cpp"
    #include "lottie/lottieitem.cpp"
    #include "lottie/lottieitem_capi.cpp"
    #include "lottie/lottiekeypath.cpp"
    #include "lottie/lottieloader.cpp"
    #include "lottie/lottiemodel.cpp"
    #include "lottie/lottieparser.cpp"
    #include "lottie/lottieproxymodel.cpp"
    #include "lottie/zip/zip.cpp"
    #include "vector/freetype/v_ft_math.cpp"
    #include "vector/freetype/v_ft_raster.cpp"
    #include "vector/freetype/v_ft_stroker.cpp"
    #include "vector/stb/stb_image.cpp"
    #include "vector/varenaalloc.cpp"
    #include "vector/vbezier.cpp"
    #include "vector/vbitmap.cpp"
    #include "vector/vbrush.cpp"
    #include "vector/vdasher.cpp"
    #include "vector/vdebug.cpp"
    #include "vector/vdrawable.cpp"
    #include "vector/vdrawhelper.cpp"
    #include "vector/vdrawhelper_common.cpp"
    // workaround to avoid redefining color_Source, color_SourceOver and src_Source
        #define color_Source sse2_color_Source
        #define color_SourceOver sse2_color_SourceOver
        #define src_Source sse2_src_Source
        #include "vector/vdrawhelper_sse2.cpp"
        #undef color_Source
        #undef color_SourceOver
        #undef src_Source
    // end workaround
    // workaround to avoid redefining color_SourceOver
        #define color_SourceOver neon_color_SourceOver
        #include "vector/vdrawhelper_neon.cpp"
        #undef color_SourceOver
    // end workaround
    #include "vector/velapsedtimer.cpp"
    #include "vector/vimageloader.cpp"
    #include "vector/vinterpolator.cpp"
    #include "vector/vmatrix.cpp"
    #include "vector/vpainter.cpp"
    #include "vector/vpath.cpp"
    #include "vector/vpathmesure.cpp"
    #include "vector/vraster.cpp"
    #include "vector/vrect.cpp"
    #include "vector/vrle.cpp"
    // #include "wasm/rlottiewasm.cpp"  // We don't need this even for WebGL builds
// end rlottie

#include "IUnityInterface.h"

extern "C" {

// Additional overloads with access to "keepAspectRatio" parameter
RLOTTIE_API void lottie_animation_render_aspect(
    Lottie_Animation_S *animation,
    size_t frame_number,
    uint32_t *buffer,
    size_t width,
    size_t height,
    size_t bytes_per_line,
    int keep_aspect
) {
    if (!animation) return;

    rlottie::Surface surface(buffer, width, height, bytes_per_line);
    animation->mAnimation->renderSync(frame_number, surface, keep_aspect);
}

RLOTTIE_API void lottie_animation_render_async_aspect(
    Lottie_Animation_S *animation,
    size_t frame_number,
    uint32_t *buffer,
    size_t width,
    size_t height,
    size_t bytes_per_line,
    int keep_aspect
) {
    if (!animation) return;

    rlottie::Surface surface(buffer, width, height, bytes_per_line);
    animation->mRenderTask = animation->mAnimation->render(frame_number, surface, keep_aspect);
    animation->mBufferRef = buffer;
}

// Unity entrypoints
void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginLoad(IUnityInterfaces * unityInterfaces) {
    lottie_init();
}

void UNITY_INTERFACE_EXPORT UNITY_INTERFACE_API UnityPluginUnload() {
    lottie_shutdown();
}

}  // extern "C"