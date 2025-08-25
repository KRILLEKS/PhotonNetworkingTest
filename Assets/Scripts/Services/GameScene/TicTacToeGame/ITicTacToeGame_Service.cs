using UniRx;

namespace Services.GameScene.TicTacToeGameController
{
   public interface ITicTacToeGame_Service
   {
      void StartTurn();
      void FinishTurn();
   }
}