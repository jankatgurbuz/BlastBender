using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BoardItems;
using NaughtyAttributes;
using Unity.Collections;
using UnityEngine;
using Util.Pool.CentralPoolHub;
using Util.SingletonSystem;

namespace Util.Pool
{
    public interface IPoolable
    {
        void Awake();
        void Create();
        void Active();
        void Inactive();
        GameObject GetGameObject();
        Transform GetTransform();
    }

    public abstract class Pool<T> : Singleton<Pool<T>> where T : IPoolable
    {
        [SerializeField]
        private T _instantiatedItem;

        [SerializeField]
        private bool _isRegisterInFactory;

        [SerializeField]
        private bool _activateOnRetrieve;

        private List<T> _activeList;
        private List<T> _inactiveList;

        private Transform _active, _inactive;

        private void Awake()
        {
            Starter();
        }
        private void Start()
        {
            if (!_isRegisterInFactory)
                return;

            PoolFactory.Instance.Register(this);
        }

        private void Starter()
        {
            _activeList = new List<T>();
            _inactiveList = new List<T>();

            CreateParent();
        }

        private void CreateParent()
        {
            _active = new GameObject("ActiveObjects").transform;
            _inactive = new GameObject("InactiveObjects").transform;

            _active.SetParent(transform);
            _inactive.SetParent(transform);
        }


        private T GetInstance()
        {
            T instance;

            if (_inactiveList.Count > 0)
            {
                instance = _inactiveList.First();
                _inactiveList.Remove(instance);
            }
            else
            {
                instance = Create();
            }

            ActivateItem(instance);
            return instance;
        }
        private void ActivateItem(T item)
        {
            if (_activateOnRetrieve)
            {
                item.GetGameObject().SetActive(true);
            }

            item.Active();
            item.GetTransform().SetParent(_active);

            _activeList.Add(item);
        }
        private T Create()
        {
            IPoolable obj = _instantiatedItem;
            var inst = Instantiate((Object)obj, _inactive) as IPoolable;
            inst.GetGameObject().SetActive(false);
            T data = (T)inst;
            data.Create();

            return data;
        }
        private void PushForInactivation(T item)
        {
            _inactiveList.Add(item);
            item.Inactive();
            item.GetGameObject().SetActive(false);
            item.GetTransform().SetParent(_inactive);
        }

        public T Retrieve()
        {
            return GetInstance();
        }
        public void Return(T data)
        {
            _activeList.Remove(data);
            PushForInactivation(data);
        }

        public void Clear()
        {
            for (int i = _activeList.Count - 1; i >= 0; i--)
            {
                Return(_activeList[i]);
            }
        }
    }
}

