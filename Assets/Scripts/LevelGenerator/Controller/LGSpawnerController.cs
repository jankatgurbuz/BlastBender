using System.Collections;
using System.Collections.Generic;
using LevelGenerator.LGPool.SpawnerPool;
using LevelGenerator.View;
using UnityEngine;

public class LGSpawnerController
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