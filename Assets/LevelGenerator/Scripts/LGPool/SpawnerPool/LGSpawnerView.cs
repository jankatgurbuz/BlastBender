using Gameplay.Pool;
using UnityEngine;

namespace LevelGenerator.View
{
    public class LGSpawnerView : MonoBehaviour, IPoolable
    {
        public Transform Transform { get; private set; }
        public GameObject GameObject { get; private set; }

        public void Awake()
        {
            GameObject = gameObject;
            Transform = transform;
        }
    }
}