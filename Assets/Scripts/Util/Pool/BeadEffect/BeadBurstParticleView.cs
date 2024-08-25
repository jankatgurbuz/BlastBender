using System.Collections.Generic;
using BoardItems;
using Cysharp.Threading.Tasks;
using Global.Controller;
using UnityEngine;
using Zenject;

namespace Util.Pool.BeadEffect
{
    public class BeadBurstParticleView : MonoBehaviour, IPoolable
    {
        private Transform _transform;
        private GameObject _gameobject;
        private Dictionary<ItemColors, BeadBurstParticleProp> _particles;
        private LayersController _layersController;
        [SerializeField] private List<BeadBurstParticleProp> _particleList;
        private const string LayerKey = "BeadBurstParticle";

        public void Awake()
        {
            _transform = transform;
            _gameobject = gameObject;
        }

        public GameObject GetGameObject()
        {
            return _gameobject;
        }

        public Transform GetTransform()
        {
            return _transform;
        }
        public void Active()
        {

        }
        public void Create()
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
        public void Inactive()
        {
            DisableAllItems();
        }

        public void Burst(ItemColors color, Vector3 position)
        {
            _transform.position = position;
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
