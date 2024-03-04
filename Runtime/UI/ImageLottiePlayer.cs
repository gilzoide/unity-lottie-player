using Unity.Jobs;
using UnityEngine;
using UnityEngine.UI;

namespace Gilzoide.LottiePlayer
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class ImageLottiePlayer : Graphic
    {
        [Space]
        [SerializeField] protected LottieAnimationAsset _animationAsset;
        [SerializeField, Min(2)] protected int _width = 128;
        [SerializeField, Min(2)] protected int _height = 128;
        [SerializeField] protected bool _loop = true;

        protected Texture2D _texture;
        protected NativeLottieAnimation _animation;
        protected float _startTime = 0;
        protected uint _currentFrame = 0;
        protected uint _lastFrame = 0;
        protected JobHandle _renderJobHandle;

        public override Texture mainTexture => _texture;

        protected override void OnEnable()
        {
            base.OnEnable();
            _startTime = Time.time;
            RecreateAnimationIfNeeded();
        }

        protected override void OnDestroy()
        {
            DestroyImmediate(_texture);
            _animation?.Dispose();
            base.OnDestroy();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            Rect pixelAdjustedRect = GetPixelAdjustedRect();
            Color32 color = this.color;
            vh.Clear();
            vh.AddVert(new Vector3(pixelAdjustedRect.xMin, pixelAdjustedRect.yMin), color, new Vector2(0f, 1f));
            vh.AddVert(new Vector3(pixelAdjustedRect.xMin, pixelAdjustedRect.yMax), color, new Vector2(0f, 0f));
            vh.AddVert(new Vector3(pixelAdjustedRect.xMax, pixelAdjustedRect.yMax), color, new Vector2(1f, 0f));
            vh.AddVert(new Vector3(pixelAdjustedRect.xMax, pixelAdjustedRect.yMin), color, new Vector2(1f, 1f));
            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(2, 3, 0);
        }

        private void RecreateAnimationIfNeeded()
        {
            if (!_animationAsset)
            {
                return;
            }

            if (!_animation.IsValid())
            {
                _animation = _animationAsset.CreateAnimation();
            }
            if (_texture == null
                || _width != _texture.width
                || _height != _texture.height)
            {
                DestroyImmediate(_texture);
                _texture = _animation.CreateTexture(_width, _height, true);
            }
        }

        void Update()
        {
            if (!Application.isPlaying || !_animation.IsValid())
            {
                return;
            }

            _currentFrame = _animation.GetFrameAtTime(Time.time - _startTime, _loop);
            if (_currentFrame != _lastFrame)
            {
                _renderJobHandle = _animation.CreateRenderJob(_currentFrame, _texture).Schedule();
            }
        }

        void LateUpdate()
        {
            if (_currentFrame != _lastFrame)
            {
                _lastFrame = _currentFrame;
                _renderJobHandle.Complete();
                _texture.Apply(true);
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (IsActive())
            {
                RecreateAnimationIfNeeded();
            }
        }
#endif
    }
}
