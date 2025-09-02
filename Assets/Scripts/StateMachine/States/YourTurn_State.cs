using Services.GameScene.Input;
using Services.GameScene.TicTacToeGameController;
using Services.ResourcesProvider;
using UniRx;
using UnityEngine;
using Zenject;
using IState = Plugins.Architecture.StateMachine.IState;

namespace StateMachine.States
{
   public class YourTurn_State : IState
   {
      public Subject<Unit> OnYourTurnStart = new Subject<Unit>();
      public Subject<Unit> OnYourTurnEnd = new Subject<Unit>();

      private ITicTacToeGame_Service _ticTacToeGame;

      [Inject]
      private void Construct(ITicTacToeGame_Service ticTacToeGameService)
      {
         _ticTacToeGame = ticTacToeGameService;
      }

      public void Enter()
      {
         OnYourTurnStart?.OnNext(Unit.Default);
         
         _ticTacToeGame.StartTurn();
      }

      public void Exit()
      {
         OnYourTurnEnd?.OnNext(Unit.Default);
         
         _ticTacToeGame.FinishTurn();
      }
   }
}