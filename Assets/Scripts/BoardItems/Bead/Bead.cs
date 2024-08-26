using Gameplay.Movement.Strategies;
using Gameplay.Pool.Bead;
using Gameplay.Pool.BoardItemPool;
using UnityEngine;

namespace BoardItems.Bead
{
    public class Bead : BaseBoardItem<BeadView>, IColorable, ISortingOrder, IMovable
    {
        [SerializeField] private ItemColors _color;
        public IMovementStrategy MovementStrategy { get; set; }
        public bool IsMoving { get; set; }

        public ItemColors Color
        {
            get => _color;
            set
            {
                _color = value;
                SetBeadColor();
            }
        }

        public Bead(int row, int column, ItemColors color) : base(row, column)
        {
            Color = color;
            IsBead = true;
            MovementStrategy = new BeadMovementStrategy();
        }

        public override void RetrieveFromPool()
        {
            base.RetrieveFromPool();
            SetBeadColor();
        }

        public override IBoardItem Copy()
        {
            return BoardItemPool.Instance.Retrieve<Bead>(Row, Column, _color);
        }

        public void SetSortingOrder(string layerKey, int row, int column)
        {
            Item?.SetSortingOrder(layerKey, row, column);
        }

        private void SetBeadColor()
        {
            if (Item != null)
            {
                Item.Color = _color;
            }
        }

        public void Pop()
        {
            Item.Blast();
        }
    }
}