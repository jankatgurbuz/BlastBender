using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util.SingletonSystem;

namespace Util.Pool.CentralPoolHub
{
    public class PoolFactory : Singleton<PoolFactory>
    {
        private readonly Dictionary<Type, object> _pools = new();

        public void Register<T>(Pool<T> pool) where T : IPoolable
        {
            var poolType = typeof(Pool<T>);
            // if (!_pools.ContainsKey(poolType))
            // {
            //     _pools.Add(poolType, pool);
            // }
            _pools.TryAdd(poolType, pool);
        }

        private Pool<T> GetPool<T>() where T : IPoolable
        {
            var poolType = typeof(Pool<T>);
            if (_pools.TryGetValue(poolType, out var pool) && pool is Pool<T> p)
            {
                return p;
            }
            return null;
        }

        public T RetrieveFromPool<T>() where T : IPoolable
        {
            var pool = GetPool<T>();
            return pool != null ? pool.Retrieve() : default;
        }
        public void ReturnToPool<T>(T item) where T : IPoolable
        {
            var pool = GetPool<T>();
            if (pool != null)
            {
                pool.Return(item);
            }
            else
            {
                Debug.LogError("No pool found for this item type.");
            }
        }
    }
}
