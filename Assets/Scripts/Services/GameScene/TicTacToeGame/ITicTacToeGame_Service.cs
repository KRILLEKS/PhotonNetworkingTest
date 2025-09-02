using StaticData.Enums;
using UniRx;

namespace Services.GameScene.TicTacToeGameController
{
   public interface ITicTacToeGame_Service
   {
      void StartTurn();
      void FinishTurn();
      (bool isWin, Marks_Enum winnerMark, bool isDraw) CheckWin();
   }
}