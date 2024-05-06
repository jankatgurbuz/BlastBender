using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LoadingScene.View
{
    [Serializable]
    public class LoadingBackgroundReferance : AssetReferenceT<GameObject>
    {
        public LoadingBackgroundReferance(string guid) : base(guid) { }
    }
}
