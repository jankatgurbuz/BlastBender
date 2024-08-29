using System;
using System.Collections.Generic;
using BoardItems;
using Cysharp.Threading.Tasks;
using Util.SingletonSystem;

namespace Gameplay.Pool.BoardItemPool
{
    public class BoardItemPool : Singleton<BoardItemPool>
    {
        private readonly Dictionary<Type, BoardItemPoolEntry> _boardItemsMap = new();
        private readonly List<IBoardItem> _pendingList = new();

       
        public bool TryRetrieveWithoutParams<TItem>(out IBoardItem item)
        {
            var typeKey = typeof(TItem);

            if (_boardItemsMap.TryGetValue(typeKey, out var boardItemPoolEntry))
            {
                return boardItemPoolEntry.TryRetrieveWithoutParams(typeKey, out item);
            }

            item = null;
            return false;
        }

        public void Return<TItem>(TItem item) where TItem : IBoardItem
        {
            if (item.IsRetrievedItem)
            {
                Pending(item);
                return;
            }

            var typeKey = item.GetType();
            Return(typeKey, item);
        }

        private void Return(Type type, IBoardItem item)
        {
            if (_boardItemsMap.TryGetValue(type, out var boardItemPoolEntry))
            {
                boardItemPoolEntry.Return(item);
            }
            else
            {
                var newItemList = new BoardItemPoolEntry();
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
                    if (pendingItem.IsRetrievedItem) continue;

                    _pendingList.RemoveAt(i);
                    Return(pendingItem.GetType(), pendingItem);
                }
            }
        }
    }
}