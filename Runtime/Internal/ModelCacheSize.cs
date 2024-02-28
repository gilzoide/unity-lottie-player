using System;

namespace Gilzoide.LottiePlayer
{
    public static class ModelCacheSize
    {
        public static uint CacheSize
        {
            get => _cacheSize;
            set
            {
                _cacheSize = value;
                RLottie.lottie_configure_model_cache_size(value);
            }
        }
        private static uint _cacheSize = 10;

        public struct Scope : IDisposable
        {
            private uint? _previousCacheSize;

            public Scope(uint cacheSize)
            {
                _previousCacheSize = CacheSize;
                CacheSize = cacheSize;
            }

            public void Dispose()
            {
                if (_previousCacheSize is uint previousCacheSize)
                {
                    CacheSize = previousCacheSize;
                    _previousCacheSize = null;
                }
            }
        }
    }
}
