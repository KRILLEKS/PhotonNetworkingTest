using Services.GameScene.Input;
using Services.GameScene.TicTacToeGameController;
using Services.ResourcesProvider;
using UnityEngine;
using Zenject;
using IState = Plugins.Architecture.StateMachine.IState;

namespace StateMachine.States
{
   public class YourTurn_State : IState
   {
      private ITicTacToeGame_Service _ticTacToeGame;
      
      [Inject]
      private void Construct(ITicTacToeGame_Service ticTacToeGameService)
      {
         _ticTacToeGame = ticTacToeGameService;
      }

      public void Enter()
      {
         _ticTacToeGame.StartTurn();
      }

      public void Exit()
      {
         _ticTacToeGame.FinishTurn();
      }
   }
}