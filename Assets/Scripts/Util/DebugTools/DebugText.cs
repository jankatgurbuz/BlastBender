using System.Linq;
using Blast.Controller;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Util.DebugTools
{
    public class DebugText
    {
        private TextMeshPro _textMeshPro;

        public static void GetText(string text, CameraController cameraController)
        {
            var debugText = new DebugText(text, cameraController);
            debugText.PlayAnim();
        }

        public DebugText(string text, CameraController cameraController)
        {
            var textObject = new GameObject("DebugText");
            textObject.transform.SetParent(cameraController.Camera.transform);
            textObject.transform.localPosition = Vector3.zero;

            _textMeshPro = textObject.AddComponent<TextMeshPro>();
            _textMeshPro.text = text;
            _textMeshPro.fontSize = 5;
            _textMeshPro.color = Color.white;
            _textMeshPro.alignment = TextAlignmentOptions.MidlineRight;
            _textMeshPro.fontStyle = FontStyles.Bold;

            _textMeshPro.sortingLayerID = GetLastSortingLayerID();
            _textMeshPro.sortingOrder = 30_000;

            _textMeshPro.GetComponent<RectTransform>().sizeDelta = new Vector2(10, 4);

            var offset = new Vector3(1, -2, 2);
            var screenPosition = cameraController.Camera.ViewportToWorldPoint(
                new Vector3(0, 1, cameraController.Camera.nearClipPlane + offset.z));
            _textMeshPro.transform.position = screenPosition + offset;
            _textMeshPro.transform.rotation = cameraController.Camera.transform.rotation;
        }

        private int GetLastSortingLayerID()
        {
            var layerIDs = SortingLayer.layers.Select(layer => layer.id).ToArray();
            return layerIDs[^1];
        }

        private async void PlayAnim()
        {
            var elapsedTime = 0f;
            var startPosition = _textMeshPro.transform.localPosition;
            var startColor = _textMeshPro.color;
            var duration = 1;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.unscaledDeltaTime*1.5f;
                var progress = elapsedTime / duration;
                
                var easedProgress = Mathf.SmoothStep(0, 1, progress);

                _textMeshPro.transform.localPosition = startPosition + Vector3.up * easedProgress * 1f;

                Color newColor = startColor;
                newColor.a = Mathf.Lerp(1, 0, progress);
                _textMeshPro.color = newColor;

                await UniTask.Delay(16, DelayType.Realtime);
            }

            Object.Destroy(_textMeshPro.gameObject);
        }
    }
}
