using UnityEditor;
using UnityEngine;

namespace Gilzoide.LottiePlayer.Editor
{
    [CustomEditor(typeof(LottieAnimationAsset))]
    public class LottieAnimationAssetEditor : UnityEditor.Editor
    {
        private Texture2D _texture;
        private NativeLottieAnimation _animation;

        void OnEnable()
        {
            _animation = ((LottieAnimationAsset) target).CreateNativeAnimation();
            _texture = _animation.CreateTexture();
        }

        void OnDisable()
        {
            _animation.Dispose();
            DestroyImmediate(_texture);
        }

        public override bool HasPreviewGUI()
        {
            return !serializedObject.isEditingMultipleObjects && _animation.IsCreated;
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            base.OnPreviewGUI(r, background);
            _animation.Render(0, _texture);
            _texture.Apply(false);
            using (new GUIMatrixScope())
            {
                GUIUtility.ScaleAroundPivot(new Vector2(1, -1), r.center);
                GUI.DrawTexture(r, _texture, ScaleMode.ScaleToFit);
            }
        }
    }
}
