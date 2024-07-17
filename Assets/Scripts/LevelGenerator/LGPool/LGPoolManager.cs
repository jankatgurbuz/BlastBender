using UnityEngine;
using Util.SingletonSystem;

namespace LevelGenerator.LGPool
{
    public class LGPoolManager : MonoBehaviour
    {
        private void Awake()
        {
            var children = GetComponentsInChildren<ISingleton>(true);

            foreach (var singleton in children)
            {
                singleton.Initialize();
            }
        }
    }
}
