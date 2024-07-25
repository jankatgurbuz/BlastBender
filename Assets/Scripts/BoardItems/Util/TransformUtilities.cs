using System.Collections.Generic;
using UnityEngine;

namespace BoardItems.Util
{
    public class TransformUtilities
    {
        private Transform _itemTransform;
        public Quaternion InitRotation { get; }
        public Vector3 InitScale { get; }

        public static implicit operator Transform(TransformUtilities utilities)
        {
            return utilities._itemTransform;
        }

        public TransformUtilities(Transform itemTransform)
        {
            _itemTransform = itemTransform;
            InitRotation = itemTransform.rotation;
            InitScale = itemTransform.localScale;
        }

        public void SetPosition(Vector3 position)
        {
            _itemTransform.position = position;
        }

        public Vector3 GetPosition()
        {
            return _itemTransform.position;
        }

        public void SetRotation(Quaternion rotation)
        {
            _itemTransform.rotation = rotation;
        }

        public void SetRotation(Vector3 rotation)
        {
            _itemTransform.rotation = Quaternion.Euler(rotation);
        }

        public Quaternion GetRotation()
        {
            return _itemTransform.rotation;
        }

        public void SetScale(Vector3 scale)
        {
            _itemTransform.localScale = scale;
        }

        public Vector3 GetScale()
        {
            return _itemTransform.localScale;
        }

        public void ResetItem()
        {
            _itemTransform.localScale = InitScale;
            _itemTransform.rotation = InitRotation;
        }
    }
}