using System.Collections;
using System.Collections.Generic;
using BoardItems;
using UnityEngine;
using System;
using BoardItems.LevelData;
using System.Linq;
using Blast.Controller;
using BoardItems.Spawner;
using BoardItems.Void;
using Zenject;
using Signals;
using Global.Controller;
using LevelGenerator.Utility;

namespace LevelGenerator.Controller
{
    public interface ILevelGeneratorController : ILGStart
    {
        public static readonly int MaxLength = 11;
        public static readonly int MinLength = 3;

        public event Action OnChangeState;
        public int RowLength { get; set; }
        public int ColumnLength { get; set; }
        public Type SelectedType { get; set; }
        public ItemColors ItemColors { get; set; }
        public TaskLocation TaskLocation { get; set; }
        public LevelData LevelData { get; }
    }

    public class LevelGeneratorController : ILevelGeneratorController
    {
        private readonly LGBorderController _borderController;
        private readonly ISpriteCanvasController _spriteCanvasController;
        private readonly ILGGridController _gridController;
        private readonly LGBoardItemController _boardItemController;

        private int _rowLength = 10;
        private int _columnLength = 10;

        private LevelData _levelData;
        private IBoardItem[,] _boardItem;
        private SpawnerData _spawnerData;

        private Type _selectedType;
        private ItemColors _itemColors;
        private TaskLocation _taskLocation;

        public event Action OnChangeState;

        public Type SelectedType
        {
            get => _selectedType;
            set => _selectedType = value;
        }

        public ItemColors ItemColors
        {
            get => _itemColors;
            set => _itemColors = value;
        }

        public TaskLocation TaskLocation
        {
            get => _taskLocation;
            set => _taskLocation = value;
        }

        public int RowLength
        {
            get => _rowLength;
            set
            {
                if (_rowLength == value)
                    return;

                _rowLength = value;
                OnChangeState?.Invoke();
            }
        }

        public int ColumnLength
        {
            get => _columnLength;
            set
            {
                if (_columnLength == value)
                    return;

                _columnLength = value;
                OnChangeState?.Invoke();
            }
        }

        public LevelData LevelData => _levelData;

        public LevelGeneratorController(SignalBus signalBus,
            LGBorderController borderController, ISpriteCanvasController spriteCanvasController,
            ILGGridController gridController, ILGGridInteractionController gridInteractionSystem,
            LGBoardItemController boardItemController)
        {
            _borderController = borderController;
            _spriteCanvasController = spriteCanvasController;
            _gridController = gridController;
            _boardItemController = boardItemController;

            _selectedType = typeof(SpaceArea);

            signalBus.Subscribe<GameStateReaction>(PlayGame);
            gridInteractionSystem.Down += Down;
        }

        private void PlayGame(GameStateReaction reaction)
        {
            switch (reaction.GameStatus)
            {
                case GameStatus.Menu:
                    _spriteCanvasController.Disable();
                    break;
                case GameStatus.Game:
                    _spriteCanvasController.Disable();
                    break;
                case GameStatus.LevelGenerator:
                    _spriteCanvasController.Enable();
                    break;
            }
        }

        public void Start()
        {
            if (_levelData == null)
            {
                _levelData = ScriptableObject.CreateInstance<LevelData>();
                _boardItem = new IBoardItem[RowLength, ColumnLength];
                _spawnerData = new()
                {
                    Spawners = new List<SpawnerPosition>()
                };
            }

            OnChangeState += ChangeState;
            OnChangeState?.Invoke();
        }

        private void Down(int row, int column)
        {
            if (row < 0 || column < 0 || row >= _rowLength || column >= _columnLength)
                return;

            ItemInstance(_boardItem, row, column);
            OnChangeState?.Invoke();
        }

        private void ChangeState()
        {
            ForeachBoardItem(_boardItem, x => x.ReturnToPool());
            _boardItemController.CreateBoardItems(ref _boardItem, _rowLength, _columnLength);
            _boardItemController.AssignBoardItem(_levelData, _boardItem);
            _borderController.CreateBorderMatrix(_levelData, _rowLength, _columnLength);
            _borderController.CreateBorder(_levelData.Border);
            _gridController.CreateIndicators();
            ForeachBoardItem(_boardItem,
                x => x.RetrieveFromPool(),
                x => x.SetPosition(_gridController.CellToLocal(x.Row, x.Column)),
                x => x.BoardVisitor?.Bead?.SetColorAndAddSprite(),
                x => x.SetActive(true));
        }


        public void ForeachBoardItem(IBoardItem[,] boardItem, params Action<IBoardItem>[] itemActions)
        {
            foreach (var item in boardItem)
            {
                if (item == null) continue;

                foreach (var action in itemActions)
                {
                    action(item);
                }
            }
        }

        private void ItemInstance(IBoardItem[,] boardItem, int row, int column)
        {
            if (TaskLocation == TaskLocation.Board)
            {
                boardItem[row, column].ReturnToPool();
                boardItem[row, column] =
                    _boardItemController.CreateInstance(_selectedType, row, column, _itemColors) as IBoardItem;

                Debug.Log(_selectedType + " <---- type");
            }
            else if (TaskLocation == TaskLocation.Spawner)
            {
                var spawnerPosition = new SpawnerPosition()
                {
                    Row = row,
                    Column = column
                };
                _spawnerData.Spawners.Add(spawnerPosition);
                _boardItemController.CreateSpawner(spawnerPosition);
            }
        }
    }
}