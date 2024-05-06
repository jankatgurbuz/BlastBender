using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util.SingletonSystem
{
    public interface ISingleton
    {
        void Initialize();
    }

    public abstract class Singleton : MonoBehaviour, ISingleton
    {
        private protected Singleton() { }
        public abstract void Initialize();
    }

    public abstract class Singleton<T> : Singleton where T : Singleton<T>
    {
        public static T Instance { get; private set; }

        public override void Initialize()
        {
            Instance = (T)this;
            Started();
        }
        public virtual void Started()
        {

        }
    }
}