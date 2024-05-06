using System.Collections;
using System.Collections.Generic;
using BoardItems;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Util.Pool.NullPtrPool
{
    public class VoidPoolView : MonoBehaviour, IPoolable, IItemBehavior
    {
        private Transform _transform;
        private GameObject _gameObject;

        public void Awake()
        {
            _transform = transform;
            _gameObject = gameObject;
        }

        public void Active()
        {
        }

        public void Create()
        {
        }

        public GameObject GetGameObject()
        {
            return _gameObject;
        }

        public Transform GetTransform()
        {
            return _transform;
        }

        public void Inactive()
        {
        }

        public void SetPosition(Vector3 position)
        {
        }

        public Vector3 GetPosition()
        {
            return _transform.position;
        }

        public void ConfigureItemProperties(ItemColors color)
        {
        }

        public void SetActive(bool active)
        {
        }

        public void SetSortingOrder(int row, int column)
        {
        }

        public void Shake()
        {
        }

        public async UniTask FinalizeMovementWithBounce()
        {
            await UniTask.CompletedTask;
        }

        public void Blast()
        {
        }
    }
}