using System;
using System.Collections;
using System.Collections.Generic;
using Gilzoide.LottiePlayer.RLottie;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace Gilzoide.LottiePlayer
{
    public readonly struct PathEnumerable : IEnumerable<Vector2>
    {
        private readonly Path _path;
        private readonly float _step;

        public PathEnumerable(Path path, float step)
        {
            if (step <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(step), "Step must be positive.");
            }
            if ((uint) path.elmCount > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(step), "Path element count is bigger than 2147483647.");
            }
            _path = path;
            _step = step;
        }

        public PathEnumerator GetEnumerator()
        {
            return new PathEnumerator(_path, _step);
        }

        IEnumerator<Vector2> IEnumerable<Vector2>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public struct PathEnumerator : IEnumerator<Vector2>
    {
        private readonly Path _path;
        private readonly float _step;

        private FlattenedBezierCurveEnumerator _curveEnumerator;
        private Vector2 _current;
        private int _elementIndex;
        private int _pointIndex;

        public PathEnumerator(Path path, float step)
        {
            if (step <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(step), "Step must be positive.");
            }
            if ((uint) path.elmCount > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(step), "Path element count is bigger than 2147483647.");
            }
            _path = path;
            _step = step;
            _curveEnumerator = default;
            _current = default;
            _elementIndex = -1;
            _pointIndex = 0;
        }

        public readonly Vector2 Current => _current;

        readonly object IEnumerator.Current => Current;

        public readonly void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (_curveEnumerator.MoveNext())
            {
                _current = _curveEnumerator.Current;
                return true;
            }
            else if (_elementIndex + 1 < unchecked((int) _path.elmCount))
            {
                _elementIndex++;
                unsafe
                {
                    switch (_path.elmPtr[_elementIndex])
                    {
                        case Path.Element.MoveTo:
                        case Path.Element.LineTo:
                            _current = _path.ptPtr[_pointIndex];
                            _pointIndex++;
                            break;

                        case Path.Element.CubicTo:
                            _curveEnumerator = new FlattenedBezierCurveEnumerator(
                                new BezierCurve(
                                    new float3(_current, 0),
                                    _path.ptPtr[_pointIndex + 0].ToFloat3(),
                                    _path.ptPtr[_pointIndex + 1].ToFloat3(),
                                    _path.ptPtr[_pointIndex + 2].ToFloat3()
                                ),
                                _step
                            );
                            _pointIndex += 3;
                            return MoveNext();
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Reset()
        {
            _curveEnumerator = default;
            _elementIndex = -1;
            _pointIndex = 0;
        }
    }
}
