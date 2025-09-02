using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Models;
using Plugins.Architecture.StateMachine;
using Plugins.Architecture.StateMachine.StateMachineFactory;
using Services.GameScene.Input;
using Services.GameScene.NetworkGameLoop_Service;
using Services.GameScene.PrefabFactory;
using Services.GameScene.TicTacToeGameController;
using Services.TicTacToeGrid;
using StateMachine;
using StateMachine.States;
using UI;
using UnityEngine;
using Zenject;

namespace Installers
{
   public class GameScene_Installer : MonoInstaller
   {
      [SerializeField] private GameLoop_Network gameLoopNetwork;
      [SerializeField] private GameLoopBridge_Network gameLoopBridgeNetwork;

      public override void InstallBindings()
      {
         Container.BindInterfacesTo<TicTacToeGrid_Service>().AsSingle();
         Container.BindInterfacesTo<Input_Service>().AsSingle();
         Container.BindInterfacesTo<TicTacToeGame_Service>().AsSingle().NonLazy();
         Container.BindInterfacesTo<GamePrefabFactory_Service>().AsSingle();

         Container.Bind<TicTacToeGame_Model>().AsSingle();
         Container.Bind<SessionData_Model>().AsSingle();

         Container.Bind<GameLoop_Network>().FromInstance(gameLoopNetwork).AsSingle();
         Container.Bind<GameLoopBridge_Network>().FromInstance(gameLoopBridgeNetwork).AsSingle();
         
         BindStateMachine();
      }

      private void BindStateMachine()
      {
         YourTurn_State yourTurnState = new YourTurn_State();
         Container.Bind<YourTurn_State>().FromInstance(yourTurnState).AsSingle();
         
         List<IBaseState> states = new List<IBaseState>()
         {
            yourTurnState,
            new WaitForOpponent_State()
         };
         
         Container.Resolve<IStateMachineFactory_Service>().SetStateMachine<GameLoop_StateMachine>(states, Container);
      }

      // It's not correct to initialize the game in the installer. Made it this way for simplicity
      public override void Start()
      {
         Container.Resolve<ITicTacToeGrid_Service>().GenerateGrid();
      }
   }
}