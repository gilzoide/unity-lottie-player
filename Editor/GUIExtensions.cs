using System;
using UnityEngine;

namespace Gilzoide.LottiePlayer.Editor
{
    public class GUIMatrixScope : IDisposable
    {
        private Matrix4x4 _matrix;

        public GUIMatrixScope()
        {
            _matrix = GUI.matrix;
        }

        public void Dispose()
        {
            GUI.matrix = _matrix;
        }
    }
}
