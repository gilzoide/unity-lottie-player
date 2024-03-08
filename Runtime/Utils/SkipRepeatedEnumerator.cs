using System;
using System.Collections;
using System.Collections.Generic;

namespace Gilzoide.LottiePlayer
{
    public struct SkipRepeatedEnumerator<T, TEnumerator> : IEnumerator<T>
        where T : struct, IEquatable<T>
        where TEnumerator : IEnumerator<T>
    {
#pragma warning disable IDE0044 // Add readonly modifier
        private TEnumerator _source;
#pragma warning restore IDE0044 // Add readonly modifier
        private T? _current;

        public SkipRepeatedEnumerator(TEnumerator enumerator)
        {
            _source = enumerator;
            _current = null;
        }

        public readonly T Current => _current.Value;

        readonly object IEnumerator.Current => Current;

        public void Dispose()
        {
            _source.Dispose();
        }

        public bool MoveNext()
        {
            while (_source.MoveNext())
            {
                T newCurrent = _source.Current;
                if (_current is not T value || !value.Equals(newCurrent))
                {
                    _current = newCurrent;
                    return true;
                }
            }
            return false;
        }

        public void Reset()
        {
            _source.Reset();
        }
    }
}
