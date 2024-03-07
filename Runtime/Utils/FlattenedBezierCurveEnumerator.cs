using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

namespace Gilzoide.LottiePlayer
{
    public readonly struct FlattenedBezierCurveEnumerable : IEnumerable<Vector3>
    {
        private readonly BezierCurve _curve;
        private readonly float _step;

        public FlattenedBezierCurveEnumerable(BezierCurve curve, float step)
        {
            _curve = curve;
            _step = step;
        }

        public readonly FlattenedBezierCurveEnumerator GetEnumerator()
        {
            return new FlattenedBezierCurveEnumerator(_curve, _step);
        }

        readonly IEnumerator<Vector3> IEnumerable<Vector3>.GetEnumerator()
        {
            return GetEnumerator();
        }

        readonly IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public struct FlattenedBezierCurveEnumerator : IEnumerator<Vector3>
    {
        private readonly BezierCurve _curve;
        private Vector3 _current;
        private int _index;
        private readonly int _stepCount;

        public FlattenedBezierCurveEnumerator(BezierCurve curve, float step)
        {
            _curve = curve;
            _stepCount = Mathf.CeilToInt(CurveUtility.ApproximateLength(curve) / step);
            _current = default;
            _index = 0;
        }

        public readonly Vector3 Current => _current;

        readonly object IEnumerator.Current => Current;

        public readonly void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (_index < _stepCount)
            {
                _index++;
                _current = CurveUtility.EvaluatePosition(_curve, (float) _index / _stepCount);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Reset()
        {
            _index = 0;
        }
    }
}
