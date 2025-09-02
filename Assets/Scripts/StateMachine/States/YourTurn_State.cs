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
      private TurnIndicator_UI _turnIndicatorUI;

      [Inject]
      private void Construct(ITicTacToeGame_Service ticTacToeGameService, TurnIndicator_UI turnIndicatorUI)
      {
         _ticTacToeGame = ticTacToeGameService;
         _turnIndicatorUI = turnIndicatorUI;
      }

      public void Enter()
      {
         _ticTacToeGame.StartTurn();
         _turnIndicatorUI.SetTurnState(true);
      }

      public void Exit()
      {
         _ticTacToeGame.FinishTurn();
         _turnIndicatorUI.SetTurnState(false);
      }
   }
}