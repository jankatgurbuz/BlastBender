using System.Collections.Generic;

namespace BoardItems.Spawner
{
    [System.Serializable]
    public class SpawnerData
    {
        public List<SpawnerPosition> Spawners;
    }

    [System.Serializable]
    public class SpawnerPosition
    {
        public int Row;
        public int Column;
    }
}