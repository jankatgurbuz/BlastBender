using System;
using System.Collections.Generic;
using System.Linq;
using BoardItems;
using Cysharp.Threading.Tasks;
using Util.SingletonSystem;

namespace Util.BoardItemPoolSystem
{
    public class BoardItemPool : Singleton<BoardItemPool>
    {
        private readonly Dictionary<Type, ItemList> _boardItemsMap = new();
        private readonly List<IBoardItem> _pendingList = new();

        public IBoardItem Retrieve<TItem>(params object[] args) where TItem : IBoardItem
        {
            var typeKey = typeof(TItem);

            if (_boardItemsMap.TryGetValue(typeKey, out ItemList itemList))
            {
                return itemList.Retrieve(typeKey, args);
            }

            var item = new ItemList();
            _boardItemsMap.Add(typeKey, item);
            return item.Retrieve(typeKey, args);
        }

        public bool TryRetrieveWithoutParams<TItem>(out IBoardItem item)
        {
            var typeKey = typeof(TItem);

            if (_boardItemsMap.TryGetValue(typeKey, out ItemList itemList))
            {
                return itemList.TryRetrieveWithoutParams(typeKey, out item);
            }

            item = null;
            return false;
        }

        public void Return<TItem>(TItem item) where TItem : IBoardItem
        {
            if (item.IsPool)
            {
                Pending(item);
                return;
            }

            var typeKey = item.GetType();
            Return(typeKey, item);
        }

        private void Return(Type type, IBoardItem item)
        {
            if (_boardItemsMap.TryGetValue(type, out ItemList itemList))
            {
                itemList.Return(item);
            }
            else
            {
                var newItemList = new ItemList();
                newItemList.Return(item);
                _boardItemsMap.Add(type, newItemList);
            }
        }

        private void Pending(IBoardItem item)
        {
            if (_pendingList.Count == 0)
            {
                _pendingList.Add(item);
                IsFinishPending();
                return;
            }

            _pendingList.Add(item);
        }

        private async void IsFinishPending()
        {
            while (_pendingList.Count != 0)
            {
                await UniTask.Delay(250);

                for (int i = _pendingList.Count - 1; i >= 0; i--)
                {
                    var pendingItem = _pendingList[i];
                    if (pendingItem.IsPool) continue;

                    _pendingList.RemoveAt(i);
                    Return(pendingItem.GetType(), pendingItem);
                }
            }
        }
    }

    public class ItemList
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