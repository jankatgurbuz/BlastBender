using LevelGenerator.LGPool.SpawnerPool;
using LevelGenerator.View;

namespace LevelGenerator.Scripts.Controller
{
    public  class LGSpawnerController
    {
        public LGSpawnerView Retrieve()
        {
            return LGSpawnerPool.Instance.Retrieve();
        }
        public void ReturnToPool()
        {
            LGSpawnerPool.Instance.Clear();
        }
    }
}