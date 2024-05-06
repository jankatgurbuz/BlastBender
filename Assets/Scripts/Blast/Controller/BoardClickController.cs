using Cysharp.Threading.Tasks;
using Global.Controller;
using Signals;
using Zenject;

namespace Blast.Controller
{
    public class BoardClickController : IStartable
    {
        private readonly IGridInteractionController _gridInteractionController;
        private readonly IInGameController _inGameController;
        private readonly BoardItemController _boardItemController;

        private int _rowLength;
        private int _columnLength;
        private bool _isActiveOnClick;

        public BoardClickController(SignalBus signalBus, IGridInteractionController gridInteractionController,
            IInGameController inGameController, BoardItemController boardItemController)
        {
            _gridInteractionController = gridInteractionController;
            _inGameController = inGameController;
            _boardItemController = boardItemController;
            signalBus.Subscribe<GameStateReaction>(PlayGame);
        }

        public async UniTask Start()
        {
            _gridInteractionController.Up += OnClick;
            await UniTask.Yield();
        }

        private void PlayGame(GameStateReaction reaction)
        {
            if (reaction.GameStatus == GameStatus.MenuInitialize)
            {
                _isActiveOnClick = false;
            }
            else if (reaction.GameStatus == GameStatus.Game)
            {
                _rowLength = _inGameController.LevelData.RowLength;
                _columnLength = _inGameController.LevelData.ColumnLength;
                _isActiveOnClick = true;
            }
        }

        private void OnClick(int row, int column)
        {
            if (!_isActiveOnClick)
                return;

            if (row < 0 || column < 0 || row >= _rowLength || column >= _columnLength)
                return;
            
            _boardItemController.OnClick(row, column);
        }
    }
}