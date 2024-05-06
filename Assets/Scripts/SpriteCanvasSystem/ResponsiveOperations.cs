using UnityEngine;

namespace SpriteCanvasSystem
{
    public interface IResponsiveOperation
    {
        public void Handle(float screenHeight, float screenWidth, Vector3 uiItemSize, Transform uiItemTransform, Vector3 referencePosition, float orthoSize, float referenceOrthoSize);
    }

    public abstract class ResponsiveOperations : IResponsiveOperation
    {
        [SerializeField] protected Vector3 LocalScale = new(1, 1, 1);

        public abstract void Handle(float screenHeight, float screenWidth,
            Vector3 uiItemSize, Transform uiItemTransform,
            Vector3 referencePosition, float orthoSize, float referenceOrthoSize);
    }


    public class TopCenter : ResponsiveOperations
    {
        [SerializeField] private float _topOffset;
        [SerializeField] private float _horizontalOffset;

        public override void Handle(float screenHeight, float screenWidth, Vector3 uiItemSize, Transform uiItemTransform, Vector3 referencePosition, float orthoSize, float referenceOrthoSize)
        {
            var balance = orthoSize / referenceOrthoSize;
            uiItemTransform.localScale = LocalScale * balance;

            var scaledHeight = uiItemSize.y * uiItemTransform.localScale.y;

            uiItemTransform.position = new Vector3(
                _horizontalOffset * balance, screenHeight * 0.5f - scaledHeight * 0.5f - _topOffset * balance, 0) + referencePosition;
        }

    }

    public class TopRight : ResponsiveOperations
    {
        [SerializeField] private float _topOffset;
        [SerializeField] private float _rightOffset;

        public override void Handle(float screenHeight, float screenWidth, Vector3 uiItemSize, Transform uiItemTransform, Vector3 referencePosition, float orthoSize, float referenceOrthoSize)
        {
            var balance = orthoSize / referenceOrthoSize;
            uiItemTransform.localScale = LocalScale * balance;

            var scaledHeight = uiItemSize.y * uiItemTransform.localScale.y;
            var scaleWidth = uiItemSize.x * uiItemTransform.localScale.x;

            uiItemTransform.position = referencePosition + new Vector3(
                screenWidth * 0.5f - scaleWidth * 0.5f - _rightOffset * balance,
                screenHeight * 0.5f - scaledHeight * 0.5f - _topOffset * balance, 0);
        }
    }

    public class TopLeft : ResponsiveOperations
    {
        [SerializeField] private float _topOffset;
        [SerializeField] private float _leftOffset;

        public override void Handle(float screenHeight, float screenWidth, Vector3 uiItemSize, Transform uiItemTransform, Vector3 referencePosition, float orthoSize, float referenceOrthoSize)
        {
            var balance = orthoSize / referenceOrthoSize;
            uiItemTransform.localScale = LocalScale * balance;

            var scaledHeight = uiItemSize.y * uiItemTransform.localScale.y;
            var scaleWidth = uiItemSize.x * uiItemTransform.localScale.x;

            uiItemTransform.position = referencePosition + new Vector3(
                scaleWidth * 0.5f - screenWidth * 0.5f + _leftOffset * balance,
                screenHeight * 0.5f - scaledHeight * 0.5f - _topOffset * balance, 0);
        }
    }

    public class TopStretch : ResponsiveOperations
    {
        [SerializeField] private float _topOffset;
        [SerializeField] private float _edgeOffset;
        [SerializeField] private float _maxSize = 10000;

        public override void Handle(float screenHeight, float screenWidth, Vector3 uiItemSize, Transform uiItemTransform, Vector3 referencePosition, float orthoSize, float referenceOrthoSize)
        {
            var balance = orthoSize / referenceOrthoSize;
            uiItemTransform.localScale = LocalScale * balance;

            var scaleRatio = screenWidth / uiItemSize.x;
            uiItemTransform.localScale = new Vector3(
                Mathf.Clamp(scaleRatio - _edgeOffset * balance, 0, _maxSize * balance),
                uiItemTransform.localScale.y, 1);

            var scaledHeight = uiItemSize.y * uiItemTransform.localScale.y;

            uiItemTransform.position = referencePosition + new Vector3(
                 0, screenHeight * 0.5f - scaledHeight * 0.5f - _topOffset * balance, 0);
        }
    }

    public class BottomCenter : ResponsiveOperations
    {
        [SerializeField] private float _bottomOffset;

        public override void Handle(float screenHeight, float screenWidth, Vector3 uiItemSize, Transform uiItemTransform, Vector3 referencePosition, float orthoSize, float referenceOrthoSize)
        {
            var balance = orthoSize / referenceOrthoSize;
            uiItemTransform.localScale = LocalScale * balance;

            var scaledHeight = uiItemSize.y * uiItemTransform.localScale.y;

            uiItemTransform.position = new Vector3(
                0, scaledHeight * 0.5f - screenHeight * 0.5f + _bottomOffset * balance, 0) + referencePosition;
        }
    }

    public class BottomRight : ResponsiveOperations
    {
        [SerializeField] private float _bottomOffset;
        [SerializeField] private float _rightOffset;

        public override void Handle(float screenHeight, float screenWidth, Vector3 uiItemSize, Transform uiItemTransform, Vector3 referencePosition, float orthoSize, float referenceOrthoSize)
        {
            var balance = orthoSize / referenceOrthoSize;
            uiItemTransform.localScale = LocalScale * balance;

            var scaledHeight = uiItemSize.y * uiItemTransform.localScale.y;
            var scaleWidth = uiItemSize.x * uiItemTransform.localScale.x;

            uiItemTransform.position = referencePosition + new Vector3(
                screenWidth * 0.5f - scaleWidth * 0.5f - _rightOffset * balance,
                scaledHeight * 0.5f - screenHeight * 0.5f + _bottomOffset * balance, 0);
        }
    }

    public class BottomLeft : ResponsiveOperations
    {
        [SerializeField] private float _bottomOffset;
        [SerializeField] private float _leftOffset;

        public override void Handle(float screenHeight, float screenWidth, Vector3 uiItemSize, Transform uiItemTransform, Vector3 referencePosition, float orthoSize, float referenceOrthoSize)
        {
            var balance = orthoSize / referenceOrthoSize;
            uiItemTransform.localScale = LocalScale * balance;

            var scaledHeight = uiItemSize.y * uiItemTransform.localScale.y;
            var scaleWidth = uiItemSize.x * uiItemTransform.localScale.x;

            uiItemTransform.position = referencePosition + new Vector3(
                scaleWidth * 0.5f - screenWidth * 0.5f + _leftOffset * balance,
                scaledHeight * 0.5f - screenHeight * 0.5f + _bottomOffset * balance, 0);
        }
    }

    public class BottomStretch : ResponsiveOperations
    {
        [SerializeField] private float _bottomOffset;
        [SerializeField] private float _edgeOffset;
        [SerializeField] private float _maxSize = 10000;

        public override void Handle(float screenHeight, float screenWidth, Vector3 uiItemSize, Transform uiItemTransform, Vector3 referencePosition, float orthoSize, float referenceOrthoSize)
        {
            var balance = orthoSize / referenceOrthoSize;
            uiItemTransform.localScale = LocalScale * balance;

            var scaleRatio = screenWidth / uiItemSize.x;
            uiItemTransform.localScale = new Vector3(
                Mathf.Clamp(scaleRatio - _edgeOffset * balance, 0, _maxSize * balance),
                uiItemTransform.localScale.y, 1);

            var scaledHeight = uiItemSize.y * uiItemTransform.localScale.y;

            uiItemTransform.position = referencePosition + new Vector3(
                 0, scaledHeight * 0.5f - screenHeight * 0.5f + _bottomOffset * balance, 0);
        }
    }
    //
    public class Center : ResponsiveOperations
    {
        [SerializeField] private float _horizontalOffset;
        [SerializeField] private float _verticalOffset;
        [SerializeField] private float _edgeOffset;


        public override void Handle(float screenHeight, float screenWidth, Vector3 uiItemSize, Transform uiItemTransform, Vector3 referencePosition, float orthoSize, float referenceOrthoSize)
        {
            var balance = orthoSize / referenceOrthoSize;
            uiItemTransform.localScale = LocalScale * balance;
            uiItemTransform.position = referencePosition + new Vector3(
             _horizontalOffset * balance,
            _verticalOffset * balance, 0);
        }
    }

    //

    public class LeftStretch : ResponsiveOperations
    {
        [SerializeField] private float _horizontalOffset;
        [SerializeField] private float _edgeOffset;
        [SerializeField] private float _maxSize = 10000;
        [SerializeField] private bool _snapCenter = false;

        public override void Handle(float screenHeight, float screenWidth, Vector3 uiItemSize, Transform uiItemTransform, Vector3 referencePosition, float orthoSize, float referenceOrthoSize)
        {
            var balance = orthoSize / referenceOrthoSize;
            uiItemTransform.localScale = LocalScale * balance;

            var scaleRatioY = screenHeight / uiItemSize.y;
            var scaleRatioX = screenWidth / uiItemSize.x;

            float x = _snapCenter ? scaleRatioX * 0.5f : uiItemTransform.localScale.x;
            uiItemTransform.localScale = new Vector3(
                x,
                Mathf.Clamp(scaleRatioY - _edgeOffset * balance, 0, _maxSize * balance), 1);

            var scaleWidth = uiItemSize.x * uiItemTransform.localScale.x;

            uiItemTransform.position = referencePosition + new Vector3(
            scaleWidth * 0.5f - screenWidth * 0.5f - _horizontalOffset * balance,
            0, 0);
        }
    }
    public class RightStretch : ResponsiveOperations
    {
        [SerializeField] private float _horizontalOffset;
        [SerializeField] private float _edgeOffset;
        [SerializeField] private float _maxSize = 10000;
        [SerializeField] private bool _snapCenter = false;

        public override void Handle(float screenHeight, float screenWidth, Vector3 uiItemSize, Transform uiItemTransform, Vector3 referencePosition, float orthoSize, float referenceOrthoSize)
        {
            var balance = orthoSize / referenceOrthoSize;
            uiItemTransform.localScale = LocalScale * balance;

            var scaleRatioY = screenHeight / uiItemSize.y;
            var scaleRatioX = screenWidth / uiItemSize.x;

            float x = _snapCenter ? scaleRatioX * 0.5f : uiItemTransform.localScale.x;
            uiItemTransform.localScale = new Vector3(
                x,
                Mathf.Clamp(scaleRatioY - _edgeOffset * balance, 0, _maxSize * balance), 1);

            var scaleWidth = uiItemSize.x * uiItemTransform.localScale.x;

            uiItemTransform.position = referencePosition + new Vector3(
            screenWidth * 0.5f - scaleWidth * 0.5f - _horizontalOffset * balance,
            0, 0);
        }
    }
}