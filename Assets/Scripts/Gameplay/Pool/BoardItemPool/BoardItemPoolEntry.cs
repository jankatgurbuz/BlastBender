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

        public IBoardItem Retrieve(Type type, object[] args)
        {
            var instance = _inactiveList.Count > 0 ? GetBoardItem() : Create(type, args);
            _activeList.Add(instance);
            return instance;
        }

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

        private IBoardItem Create(Type type, params object[] args)
        {
            return (IBoardItem)Activator.CreateInstance(type, args);
        }
    }
}