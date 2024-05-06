using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Util.SingletonSystem
{
    public static class SingletonLoader 
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            var singletons = Resources.Load<GameObject>("Singletons");
            var instance = Object.Instantiate(singletons);
            var children = instance.GetComponentsInChildren<ISingleton>(true);

            foreach (var singleton in children)
            {
                singleton.Initialize();
            }

            Object.DontDestroyOnLoad(instance);
            instance.transform.DetachChildren();
            Object.Destroy(instance);
        }
    }
}
