using System.Collections.Generic;
using BoardItems;
using UnityEngine;
using System;
using BoardItems.LevelData;
using System.Linq;
using System.Reflection;
using Blast.Controller;
using BoardItems.Bead;
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
        public const int MaxLength = 11;
        public const int MinLength = 3;
        public event Action OnChangeState;
        public void Initialize();
        public int RowLength { get; set; }
        public int ColumnLength { get; set; }
        public Type SelectedType { get; set; }
        public ItemColors ItemColors { get; set; }
        public TaskLocation TaskLocation { get; set; }
        public LevelData LevelData { get; }
        public IBoardItem[,] BoardItem { get; set; }
    }

    public class LevelGeneratorController : ILevelGeneratorController
    {
        private readonly ISpriteCanvasController _spriteCanvasController;
        private readonly LGGridController _gridController;
        private readonly ILGGridInteractionController _gridInteractionController;
        private readonly LGSpawnerController _spawnerController;

        private int _rowLength = 10;
        private int _columnLength = 10;

        private SpawnerData _spawnerData;
        public Type SelectedType { get; set; } = typeof(SpaceArea);
        public ItemColors ItemColors { get; set; }
        public TaskLocation TaskLocation { get; set; }
        public IBoardItem[,] BoardItem { get; set; }
        public LevelData LevelData { get; private set; }

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

        public event Action OnChangeState;

        public LevelGeneratorController(SignalBus signalBus, ISpriteCanvasController spriteCanvasController,
            LGGridController gridController, ILGGridInteractionController gridInteractionSystem,
            LGSpawnerController spawnerController)
        {
            _spriteCanvasController = spriteCanvasController;
            _gridController = gridController;
            _gridInteractionController = gridInteractionSystem;
            _spawnerController = spawnerController;
            signalBus.Subscribe<GameStateReaction>(PlayGame);
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
            if (LevelData == null)
            {
                LevelData = ScriptableObject.CreateInstance<LevelData>();
                BoardItem = new IBoardItem[RowLength, ColumnLength];
                _spawnerData = new()
                {
                    Spawners = new List<SpawnerPosition>()
                };

                LevelData.SpawnerData = _spawnerData;
            }

            OnChangeState += ChangeState;
            _gridInteractionController.Down += Down;
        }

        public void Initialize()
        {
            OnChangeState?.Invoke();
        }

        private void Down(int row, int column)
        {
            ItemInstance(row, column);
            OnChangeState?.Invoke();
        }

        private void ChangeState()
        {
            IterateBoardItem(BoardItem, x => x.ReturnToPool());
            _spawnerController.ReturnToPool();
            CreateBoardItems();
            CreateSpawner();
            AssignBoardItem();
            IterateBoardItem(BoardItem,
                x => x.RetrieveFromPool(),
                x => x.TransformUtilities?.SetPosition(_gridController.CellToLocal(x.Row, x.Column)),
                x => {
                    if (x is Bead bead)
                    {
                        bead.SetColorAndAddSprite();
                    }
                },
                x => x.SetActive(true));
        }


        public void IterateBoardItem(IBoardItem[,] boardItem, params Action<IBoardItem>[] itemActions)
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

        private void ItemInstance(int row, int column)
        {
            if (TaskLocation == TaskLocation.Board)
            {
                if (row < 0 || column < 0 || row >= _rowLength || column >= _columnLength)
                    return;

                BoardItem[row, column].ReturnToPool();
                BoardItem[row, column] = CreateInstance(SelectedType, row, column, ItemColors) as IBoardItem;
            }
            else if (TaskLocation == TaskLocation.Spawner)
            {
                if (column < 0 || column >= _columnLength) return;

                var item = _spawnerData.Spawners.Find(x => x.Row == row && x.Column == column);

                if (item != null) return;
                
                var spawnerPosition = new SpawnerPosition()
                {
                    Row = row,
                    Column = column
                };
                _spawnerData.Spawners.Add(spawnerPosition);
            }
        }

        private void CreateBoardItems()
        {
            var tempBoardItem = new IBoardItem[RowLength, ColumnLength];


            for (int i = 0; i < RowLength; i++)
            {
                if (i >= BoardItem.GetLength(0))
                    continue;

                for (int j = 0; j < ColumnLength; j++)
                {
                    if (j >= BoardItem.GetLength(1))
                        continue;

                    tempBoardItem[i, j] = BoardItem[i, j];
                }
            }

            BoardItem = tempBoardItem;

            for (int i = 0; i < RowLength; i++)
            {
                for (int j = 0; j < ColumnLength; j++)
                {
                    if (BoardItem[i, j] == null)
                    {
                        BoardItem[i, j] = CreateInstance<VoidArea>(i, j);
                    }
                }
            }
        }

        public object CreateInstance(Type type, params object[] constructorArgs)
        {
            ConstructorInfo[] constructors = type.GetConstructors();
            if (constructors.Length != 0)
            {
                ParameterInfo[] paramsInfo = constructors[0].GetParameters();
                if (paramsInfo.Length < constructorArgs.Length)
                {
                    var newConst = new object[paramsInfo.Length];
                    for (int i = 0; i < newConst.Length; i++)
                        newConst[i] = constructorArgs[i];
                    constructorArgs = newConst;
                }
            }

            var instance = Activator.CreateInstance(type, constructorArgs);
            return instance;
        }

        private void AssignBoardItem()
        {
            LevelData.BoardItem = BoardItem.Cast<IBoardItem>().ToArray();
            LevelData.RowLength = BoardItem.GetLength(0);
            LevelData.ColumnLength = BoardItem.GetLength(1);
        }

        private T CreateInstance<T>(params object[] constructorArgs) where T : IBoardItem
        {
            return (T)CreateInstance(typeof(T), constructorArgs);
        }


        private void CreateSpawner()
        {
            foreach (var spawnerDataSpawner in LevelData.SpawnerData.Spawners)
            {
                var item = _spawnerController.Retrieve();
                item.transform.position =
                    _gridController.CellToLocal(spawnerDataSpawner.Row, spawnerDataSpawner.Column);
            }
        }
    }
}