using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using Models;
using Plugins.Architecture.Extensions;
using StateMachine;
using StateMachine.States;
using StaticData.Enums;
using UnityEngine;
using Zenject;

namespace Services.GameScene.NetworkGameLoop_Service
{
   public class GameLoop_Network : NetworkBehaviour
   {
      public bool IsMyTurn
      {
         get
         {
            return Runner.LocalPlayer == TurnOrder[CurrentTurnIndex];
         }
      }

      private GameLoop_StateMachine _gameLoopStateMachine;
      private TicTacToeGame_Model _ticTacToeGameModel;
      private SessionData_Model _sessionDataModel;

      [Networked] private int CurrentTurnIndex { get; set; }
      private List<PlayerRef> TurnOrder { get; set; }


      [Inject]
      private void Construct(GameLoop_StateMachine gameLoopStateMachine, TicTacToeGame_Model ticTacToeGameModel, SessionData_Model sessionDataModel)
      {
         _gameLoopStateMachine = gameLoopStateMachine;
         _ticTacToeGameModel = ticTacToeGameModel;
         _sessionDataModel = sessionDataModel;
      }

      public override void Spawned()
      {
         if (Object.HasStateAuthority)
         {
            InitializeGame();
            StartGame();
         }
      }

      private void InitializeGame()
      {
         // Get all players and create random turn order
         var players = new List<PlayerRef>(Runner.ActivePlayers);
         players.Shuffle(); // Randomize turn order

         TurnOrder = new List<PlayerRef>();
         foreach (var player in players)
            TurnOrder.Add(player);

         if (TurnOrder.Count == 2)
         {
            RPC_SetMark(TurnOrder[0], Marks.Cross);
            RPC_SetMark(TurnOrder[1], Marks.Circle);
         }
         // for testing only
         else
            foreach (var player in TurnOrder)
               RPC_SetMark(player, Marks.Cross);


         // Set initial turn to first player
         CurrentTurnIndex = 0;
      }

      public void StartGame()
      {
         // Initialize player states
         foreach (var player in TurnOrder)
         {
            if (player == TurnOrder[CurrentTurnIndex])
               RPC_StartTurn(player);
            else
               RPC_FinishTurn(player);
         }

         RPC_TurnChanged(CurrentTurnIndex);
      }

      public void PlayerFinishedTurn()
      {
         if (!IsMyTurn || !Object.HasStateAuthority)
            return;

         // Move to next player's turn
         RPC_FinishTurn(Runner.LocalPlayer);

         CurrentTurnIndex = (CurrentTurnIndex + 1) % TurnOrder.Count;

         // Set next player's state to YourTurn
         var nextPlayer = TurnOrder[CurrentTurnIndex];
         RPC_StartTurn(nextPlayer);

         RPC_TurnChanged(CurrentTurnIndex);
      }

      [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
      private void RPC_SetMark(PlayerRef playerRef, Marks mark)
      {
         _sessionDataModel.Marks = mark;
      }

      [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
      private void RPC_TurnChanged(int turnIndex)
      {
      }

      [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
      private void RPC_FinishTurn(PlayerRef player)
      {
         Debug.Log("Finish turn! " + player);
         if (player != TurnOrder[CurrentTurnIndex])
            Debug.LogWarning("User tried to finish not it's turn");
         _gameLoopStateMachine.Enter<WaitForOpponent_State>();
      }

      [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
      private void RPC_StartTurn(PlayerRef player)
      {
         Debug.Log("Start turn! " + player);
         _gameLoopStateMachine.Enter<YourTurn_State>();
      }

      [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
      public void RPC_PlaceMark(int x, int y, Marks mark)
      {
         _ticTacToeGameModel.SetMark(x, y, mark);
      }
   }
}