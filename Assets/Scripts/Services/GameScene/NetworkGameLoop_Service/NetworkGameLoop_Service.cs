using System;
using System.Collections.Generic;
using Fusion;
using Plugins.Architecture.Extensions;
using StateMachine;
using StateMachine.States;
using UnityEngine;
using Zenject;

namespace Services.GameScene.NetworkGameLoop_Service
{
   public class NetworkGameLoop_Service : NetworkBehaviour
   {
      public bool IsMyTurn
      {
         get
         {
            return Runner.LocalPlayer == TurnOrder[CurrentTurnIndex];
         }
      }

      private GameLoop_StateMachine _gameLoopStateMachine;

      [Networked] private int CurrentTurnIndex { get; set; }
      private NetworkLinkedList<PlayerRef> TurnOrder { get; set; }


      // [Inject]
      private void Construct(GameLoop_StateMachine gameLoopStateMachine)
      {
         _gameLoopStateMachine = gameLoopStateMachine;
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
         Debug.Log(Runner);
         Debug.Log(Runner.ActivePlayers);

         // Get all players and create random turn order
         var players = new List<PlayerRef>(Runner.ActivePlayers);
         players.Shuffle(); // Randomize turn order

         TurnOrder = new NetworkLinkedList<PlayerRef>();
         foreach (var player in players)
            TurnOrder.Add(player);

         // Set initial turn to first player
         CurrentTurnIndex = 0;
      }

      private void StartGame()
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
      private void RPC_TurnChanged(int turnIndex)
      {
      }

      [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
      private void RPC_FinishTurn(PlayerRef player)
      {
         if (player == Runner.LocalPlayer)
            _gameLoopStateMachine.Enter<WaitForOpponent_State>();
         else
            Debug.LogWarning("You want to finish turn for other player");
      }

      [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
      private void RPC_StartTurn(PlayerRef player)
      {
         if (player != Runner.LocalPlayer)
            _gameLoopStateMachine.Enter<WaitForOpponent_State>();
         else
            Debug.LogWarning("You want to start turn for local player");
      }
   }
}