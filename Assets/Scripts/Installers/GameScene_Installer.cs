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
using UnityEngine;
using Zenject;

namespace Installers
{
   public class GameScene_Installer : MonoInstaller
   {
      [SerializeField] private GameLoop_Network gameLoopNetwork;

      public override void InstallBindings()
      {
         Container.BindInterfacesTo<TicTacToeGrid_Service>().AsSingle();
         Container.BindInterfacesTo<Input_Service>().AsSingle();
         Container.BindInterfacesTo<TicTacToeGame_Service>().AsSingle().NonLazy();
         Container.BindInterfacesTo<GamePrefabFactory_Service>().AsSingle();

         Container.Bind<TicTacToeGame_Model>().AsSingle();
         Container.Bind<SessionData_Model>().AsSingle();

         Container.Bind<GameLoop_Network>().FromInstance(gameLoopNetwork);
         
         BindStateMachine();
      }

      private void BindStateMachine()
      {
         // await UniTask.WaitForSeconds(0.5f);
         List<IBaseState> states = new List<IBaseState>()
         {
            new YourTurn_State(),
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