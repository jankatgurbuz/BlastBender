using System;
using System.Collections.Generic;
using System.Linq;
using BoardItems;
using BoardItems.Bead;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Util.SingletonSystem;

namespace BoardItemPoolSystem
{
    public class BoardItemPool : Singleton<BoardItemPool>
    {
        private Dictionary<Type, ItemList> _boardItemsMap = new();
        private readonly List<IBoardItem> _pendingList = new();

        public TItem Retrieve<TItem>(params object[] args) where TItem : IBoardItem
        {
            var typeKey = typeof(TItem);

            if (_boardItemsMap.TryGetValue(typeKey, out ItemList itemList))
            {
                return (TItem)itemList.Retrieve(args);
            }

            var item = new ItemList(typeKey);
            _boardItemsMap.Add(typeKey, item);
            return (TItem)item.Retrieve(args);
        }

        public void Return<TItem>(TItem item) where TItem : IBoardItem
        {
            if (item.IsPool)
            {
                Pending(item);
                return;
            }


            var typeKey = item.GetType();
            // Debug.Log("Return Type:" + typeof(TItem));
            if (_boardItemsMap.TryGetValue(typeKey, out ItemList itemList))
            {
                itemList.Return(item);
            }
            else
            {
                var newItemList = new ItemList(typeKey);
                newItemList.Return(item);
                _boardItemsMap.Add(typeKey, newItemList);
            }
        }

        public void Return(Type type, IBoardItem item)
        {
            if (_boardItemsMap.TryGetValue(type, out ItemList itemList))
            {
                itemList.Return(item);
            }
            else
            {
                var newItemList = new ItemList(type);
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
                await UniTask.Delay(200);

                for (int i = _pendingList.Count - 1; i >= 0; i--)
                {
                    var pendingItem = _pendingList[i];
                    // Debug.Log(_pendingList.Count + "--- row -> " + pendingItem.Row + "---- column " +
                    //           pendingItem.Column + "--- i:" + i);
                    if (pendingItem.IsPool) continue;

                    _pendingList.RemoveAt(i);
                    Return(pendingItem.GetType(), pendingItem);
                }
            }
        }

        public class ItemList
        {
            private readonly Type _itemType;
            private readonly List<IBoardItem> _activeList = new();
            private readonly List<IBoardItem> _inactiveList = new();

            public ItemList(Type itemType)
            {
                _itemType = itemType;
            }

            private IBoardItem GetInstance(object[] args)
            {
                IBoardItem instance;

                if (_inactiveList.Count > 0)
                {
                    instance = _inactiveList.First();
                    _inactiveList.Remove(instance);
                }
                else
                {
                    instance = Create(_itemType, args);
                }

                ActivateItem(instance);
                return instance;
            }

            private void ActivateItem(IBoardItem item)
            {
                // item.Active();
                _activeList.Add(item);
            }

            private IBoardItem Create(Type type, params object[] args)
            {
                return BoardItemFactory.CreateAnimal(type, args);
            }

            private void PushForInactivation(IBoardItem item)
            {
                _inactiveList.Add(item);
                // item.Inactive();
            }

            public IBoardItem Retrieve(object[] args)
            {
                var instance = GetInstance(args);
                return instance;
            }

            public void Return(IBoardItem item)
            {
                _activeList.Remove(item);
                PushForInactivation(item);
            }
        }
    }


    public static class BoardItemFactory
    {
        public static IBoardItem CreateAnimal(Type type, params object[] args)
        {
            return (IBoardItem)Activator.CreateInstance(type, args);
        }
    }
}