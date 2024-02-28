using UnityEngine;

namespace Gilzoide.LottiePlayer
{
    public class LottieAnimationAsset : ScriptableObject
    {
        [SerializeField] private string _cacheKey = "";
        [SerializeField] private string _resourcePath = "";
        [SerializeField, HideInInspector] private string _json;

        /// <summary>
        /// Key used by rlottie cache system.
        /// </summary>
        public string CacheKey
        {
            get => _cacheKey ?? "";
            set => _cacheKey = value ?? "";
        }

        /// <summary>
        /// Key used by rlottie cache system.
        /// </summary>
        public string ResourcePath
        {
            get => _resourcePath ?? "";
            set => _resourcePath = value ?? "";
        }

        /// <summary>
        /// Bytes that compose the Lottie JSON content.
        /// </summary>
        public string Json
        {
            get => _json;
            set => _json = value ?? "";
        }

        public LottieAnimation CreateAnimation()
        {
            return new LottieAnimation(Json, CacheKey, ResourcePath);
        }

        public NativeLottieAnimation CreateNativeAnimation()
        {
            return new NativeLottieAnimation(Json, CacheKey, ResourcePath);
        }
    }
}
