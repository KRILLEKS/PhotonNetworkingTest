using Plugins.Architecture.StateMachine;
using Services.GameScene.TicTacToeGameController;
using Zenject;

namespace StateMachine.States
{
   public class WaitForOpponent_State : IState
   {
      private ITicTacToeGame_Service _ticTacToeGameService;
      
      [Inject]
      private void Construct(ITicTacToeGame_Service ticTacToeGameService)
      {
         _ticTacToeGameService = ticTacToeGameService;
      }

      public void Enter()
      {
         _ticTacToeGameService.StartTurn();
      }

      public void Exit()
      {
         _ticTacToeGameService.FinishTurn();
      }
   }
}