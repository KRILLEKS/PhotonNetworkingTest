using System.Collections.Generic;
using Plugins.Architecture.StateMachine;
using Plugins.Architecture.StateMachine.StateMachineFactory;
using Services.GameScene.Input;
using Services.GameScene.TicTacToeGameController;
using Services.TicTacToeGrid;
using StateMachine;
using StateMachine.States;
using Zenject;

namespace Installers
{
   public class GameScene_Installer : MonoInstaller
   {
      public override void InstallBindings()
      {
         Container.BindInterfacesTo<TicTacToeGrid_Service>().AsSingle();
         Container.BindInterfacesTo<Input_Service>().AsSingle();
         Container.BindInterfacesTo<TicTacToeGameController_Service>().AsSingle();
         
         BindStateMachine();
      }

      private void BindStateMachine()
      {
         List<IBaseState> states = new List<IBaseState>()
         {
            new YourTurn_State(),
            new WaitForOpponent_State()
         };
         Container.Resolve<IStateMachineFactory_Service>().SetStateMachine<GameLoop_StateMachine>(states);
      }

      // It's not correct to initialize the game in the installer. Made it this way for simplicity
      public override void Start()
      {
         Container.Resolve<ITicTacToeGrid_Service>().GenerateGrid();
      }
   }
}