using System.Collections;
using System.Collections.Generic;

namespace Gilzoide.LottiePlayer
{
    public struct PairOfPairsEnumerator<T, TEnumerator> : IEnumerator<(T?, T?, T?)>
        where T : struct
        where TEnumerator : IEnumerator<T>
    {
#pragma warning disable IDE0044 // Add readonly modifier
        private TEnumerator _source;
#pragma warning restore IDE0044 // Add readonly modifier
        private (T?, T?, T?) _current;

        public PairOfPairsEnumerator(TEnumerator enumerator)
        {
            _source = enumerator;
            _current = (null, null, null);
        }

        public readonly (T?, T?, T?) Current => _current;

        readonly object IEnumerator.Current => Current;

        public void Dispose()
        {
            _source.Dispose();
        }

        public bool MoveNext()
        {
            switch (_current)
            {
                case (null, null, null):
                    if (!_source.MoveNext())
                    {
                        return false;
                    }
                    T first = _source.Current;
                    if (!_source.MoveNext())
                    {
                        return false;
                    }
                    _current = (null, first, _source.Current);
                    break;

                case (_, T t2, T t3):
                    T? nextValue = _source.MoveNext() ? _source.Current : null;
                    _current = (t2, t3, nextValue);
                    break;

                case (_, _, null):
                    return false;
            }
            return true;
        }

        public void Reset()
        {
            _source.Reset();
        }
    }

    public static class PairOfPairsEnumeratorExtensions
    {
        public static PairOfPairsEnumerator<T, TEnumerator> PairOfPairs<T, TEnumerator>(this TEnumerator enumerator)
            where T : struct
            where TEnumerator : IEnumerator<T>
        {
            return new PairOfPairsEnumerator<T, TEnumerator>(enumerator);
        }
    }
}
