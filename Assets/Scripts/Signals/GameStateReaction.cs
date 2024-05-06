using Global.Controller;

namespace Signals
{
    public class GameStateReaction
    {
        public readonly GameStatus GameStatus;
        public GameStateReaction(GameStatus gameStatus)
        {
            GameStatus = gameStatus;
        }
    }
}
