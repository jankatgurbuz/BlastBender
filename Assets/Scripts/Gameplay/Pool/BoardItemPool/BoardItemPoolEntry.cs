using System;
using System.Collections.Generic;
using System.Linq;
using BoardItems;

namespace Gameplay.Pool.BoardItemPool
{
    public class BoardItemPoolEntry
    {
        private readonly List<IBoardItem> _activeList = new();
        private readonly List<IBoardItem> _inactiveList = new();

        public bool TryRetrieveWithoutParams(Type type, out IBoardItem item)
        {
            if (_inactiveList.Count > 0)
            {
                item = GetBoardItem();
                return true;
            }

            item = null;
            return false;
        }

        public void Return(IBoardItem item)
        {
            _activeList.Remove(item);
            _inactiveList.Add(item);
        }

        private IBoardItem GetBoardItem()
        {
            var instance = _inactiveList.First();
            _inactiveList.Remove(instance);
            return instance;
        }
    }
}