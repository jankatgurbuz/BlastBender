using UnityEngine;
using NaughtyAttributes;
using UnityEngine.U2D.Animation;
using TMPro;

namespace SpriteCanvasSystem
{
    [ExecuteInEditMode]
    public class SpriteCanvas : MonoBehaviour
    {
        [SerializeField, Required("Camera cannot be null!")]
        private Camera _camera;

        [SerializeField]
        private float _referenceOrthographicSize = 5;

        [SerializeField, SortingLayer]
        private string _sortingLayerName;

        [SerializeField]
        private int _sortingLayerOrder;

        [SerializeField, InfoBox("Runtime performance may decrease when active!")]
        private bool _isActiveDuringRuntime;

        [SerializeField, Range(0f, 1f)]
        private float _uiAlpha = 1f;

        [SerializeField]
        private bool _interactable = true;

        [ShowNonSerializedField]
        private float _screenHeight;

        [ShowNonSerializedField]
        private float _screenWidth;

        public float ScreenHeight { get => _screenHeight; }
        public float ScreenWidth { get => _screenWidth; }

        private UIElement[] _spritesUI;

        private void Start()
        {
            Adjust();
        }
        public void Adjust()
        {
            if (_camera == null)
            {
                Debug.LogError("Camera cannot be null!");
                return;
            }
            AdjustSize();
            InitializeAllUISprite();
            ArrangeSpritesAlpha();
            ArrangeInteractable();
        }

        private void AdjustSize()
        {
            _screenHeight = _camera.orthographicSize * 2f;
            _screenWidth = _screenHeight * _camera.aspect;
        }

        private void InitializeAllUISprite()
        {
            _spritesUI = GetComponentsInChildren<UIElement>();
            foreach (var item in _spritesUI)
            {
                item.Initialize(_camera);
                item.ArrangeLayers(_sortingLayerName, _sortingLayerOrder);
            }
        }

        private void ArrangeSpritesAlpha()
        {
            static Color SetAlpha(Color c, float a) => new(c.r, c.g, c.b, a);

            foreach (var item in GetComponentsInChildren<SpriteRenderer>(true))
            {
                item.color = SetAlpha(item.color, _uiAlpha);
            }
            foreach (var item in GetComponentsInChildren<TextMeshPro>(true))
            {
                item.color = SetAlpha(item.color, _uiAlpha);
            }
        }
        private void ArrangeInteractable()
        {
            _spritesUI = GetComponentsInChildren<UIElement>();
            foreach (var item in _spritesUI)
            {
                item.Interactable = _interactable;
            }
        }

        private void LateUpdate()
        {
            PlayEditor();
            foreach (var item in _spritesUI)
            {
                item.Handle(_screenHeight, _screenWidth,
                    _camera, _referenceOrthographicSize);
            }
        }
        private void PlayEditor()
        {
            if (_isActiveDuringRuntime && Application.isPlaying)
                Adjust();

#if UNITY_EDITOR
            if (!Application.isPlaying)
                Adjust();
#endif
        }

        public void EnableUI()
        {
            _interactable = true;
            _uiAlpha = 1;
            ArrangeSpritesAlpha();
            ArrangeInteractable();
        }

        public void DisableUI()
        {
            _interactable = false;
            _uiAlpha = 0;
            ArrangeSpritesAlpha();
            ArrangeInteractable();
        }

    }
}
