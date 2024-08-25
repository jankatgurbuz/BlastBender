using System.Collections.Generic;
using BoardItems;
using Cysharp.Threading.Tasks;
using Global.Controller;
using UnityEngine;
using Zenject;

namespace Util.Pool.BeadEffect
{
    public class BeadBurstParticleView : MonoBehaviour, IPoolable, IInitializable, IDeactivatable
    {
        private const string LayerKey = "BeadBurstParticle";
        
        [SerializeField] private List<BeadBurstParticleProp> _particleList;
        
        private Dictionary<ItemColors, BeadBurstParticleProp> _particles;
        private LayersController _layersController;
        public GameObject GameObject { get; private set; }
        public Transform Transform { get; private set; }

        public void Awake()
        {
            Transform = transform;
            GameObject = gameObject;
        }
        public void Initialize()
        {
            _layersController ??= ProjectContext.Instance.Container.Resolve<LayersController>();
            var info = _layersController.GetLayerInfo(LayerKey);

            _particles = new Dictionary<ItemColors, BeadBurstParticleProp>();

            foreach (var item in _particleList)
            {
                _particles.Add(item.ItemColor, item);
                item.ParticleSystemRenderer.GetComponent<ParticleSystemRenderer>().sortingLayerID = info.SortingLayer;
                item.ParticleSystemRenderer.GetComponent<ParticleSystemRenderer>().sortingOrder = info.OrderInLayer;
            }

            DisableAllItems();
        }

        public void Deactivate()
        {
            DisableAllItems();
        }

        public void Burst(ItemColors color, Vector3 position)
        {
            Transform.position = position;
            _particles[color].ParticleObject.SetActive(true);
            WaitForSecond();
        }

        private async void WaitForSecond()
        {
            await UniTask.Delay(2000);
            BeadBurstParticlePool.Instance.Return(this);
        }

        private void DisableAllItems()
        {
            foreach (var item in _particles)
            {
                item.Value.ParticleObject.SetActive(false);
            }
        }

        [System.Serializable]
        public class BeadBurstParticleProp
        {
            public ItemColors ItemColor;
            public GameObject ParticleObject;
            public ParticleSystem ParticleSystemRenderer;
        }
    }
}