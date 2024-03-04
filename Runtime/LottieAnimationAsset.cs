using UnityEngine;

namespace Gilzoide.LottiePlayer
{
    public class LottieAnimationAsset : ScriptableObject
    {
        [SerializeField] private Vector2Int _size;
        [SerializeField] private uint _frameCount;
        [SerializeField] private double _frameRate;
        [SerializeField] private double _duration;

        [Space]
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

        public Vector2Int Size => _size;
        public uint FrameCount => _frameCount;
        public double FrameRate => _frameRate;
        public double Duration => _duration;

        public LottieAnimation CreateAnimation()
        {
            return new LottieAnimation(Json, CacheKey, ResourcePath);
        }

        public NativeLottieAnimation CreateNativeAnimation()
        {
            return new NativeLottieAnimation(Json, CacheKey, ResourcePath);
        }

        public bool UpdateMetadata()
        {
            using (var animation = CreateNativeAnimation())
            if (animation.IsCreated)
            {
                _size = animation.GetSize();
                _frameCount = animation.GetTotalFrame();
                _frameRate = animation.GetFrameRate();
                _duration = animation.GetDuration();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
