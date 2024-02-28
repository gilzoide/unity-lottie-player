using UnityEngine;

namespace Gilzoide.LottiePlayer
{
    public class LottieAnimationAsset : ScriptableObject
    {
        [SerializeField] private string _cacheKey = "";
        [SerializeField] private string _resourcePath = "";
        [SerializeField, HideInInspector] private byte[] _bytes;

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
        public byte[] Bytes
        {
            get => _bytes;
            set
            {
                if (value == null || value.Last() == 0)
                {
                    _bytes = value;
                }
                else
                {
                    _bytes = new byte[value.Length + 1];
                    value.CopyTo(_bytes, 0);
                    _bytes.LastRef() = 0;
                }
            }
        }

        public LottieAnimation CreateAnimation()
        {
            unsafe
            {
                fixed (byte* ptr = _bytes)
                {
                    return new LottieAnimation(ptr, CacheKey, ResourcePath);
                }
            }
        }
    }
}
