using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using Models;
using Plugins.Architecture.Extensions;
using Services.GameScene.TicTacToeGameController;
using StateMachine;
using StateMachine.States;
using StaticData;
using StaticData.Enums;
using UI;
using UnityEngine;
using Zenject;

namespace Services.GameScene.NetworkGameLoop_Service
{
   // Controls game loop
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
      private SessionData_Model _sessionDataModel;
      private ITicTacToeGame_Service _ticTacToeGameService;
      private TurnIndicator_UI _turnIndicatorUI;
      private GameEnd_UI _gameEndUI;

      [Networked] private int CurrentTurnIndex { get; set; }
      private List<PlayerRef> TurnOrder { get; set; } // only host has this info


      [Inject]
      private void Construct(GameLoop_StateMachine gameLoopStateMachine,
                             SessionData_Model sessionDataModel,
                             ITicTacToeGame_Service ticTacToeGameService,
                             GameEnd_UI gameEndUI,
                             TurnIndicator_UI turnIndicatorUI)
      {
         _gameLoopStateMachine = gameLoopStateMachine;
         _sessionDataModel = sessionDataModel;
         _ticTacToeGameService = ticTacToeGameService;
         _gameEndUI = gameEndUI;
         _turnIndicatorUI = turnIndicatorUI;
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
            RPC_SetSessionData(TurnOrder[0], Marks.Cross);
            RPC_SetSessionData(TurnOrder[1], Marks.Circle);
         }
         // for testing only
         else
            foreach (var player in TurnOrder)
               RPC_SetSessionData(player, Marks.Cross);


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
      }

      [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
      private void RPC_SetSessionData([RpcTarget] PlayerRef playerRef, Marks mark)
      {
         _sessionDataModel.Mark = mark;

         _turnIndicatorUI.SetMarkImage(mark);
      }

      [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
      public void RPC_PlayerFinishedTurn()
      {
         // Move to next player's turn
         RPC_FinishTurn(TurnOrder[CurrentTurnIndex]);

         CurrentTurnIndex = (CurrentTurnIndex + 1) % TurnOrder.Count;

         // Set next player's state to YourTurn
         var nextPlayer = TurnOrder[CurrentTurnIndex];
         RPC_StartTurn(nextPlayer);
      }

      [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
      private void RPC_FinishTurn([RpcTarget] PlayerRef player)
      {
         Debug.Log("Finish turn! " + player);
         _gameLoopStateMachine.Enter<WaitForOpponent_State>();
      }

      [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
      private void RPC_StartTurn([RpcTarget] PlayerRef player)
      {
         Debug.Log("Start turn! " + player);
         _gameLoopStateMachine.Enter<YourTurn_State>();
      }

      [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
      public void RPC_CheckWin()
      {
         (bool isWin, Marks winnerMark, bool isDraw) winInfo = _ticTacToeGameService.CheckWin();
         if (winInfo.isWin)
            RPC_Win(winInfo.winnerMark);
         else if (winInfo.isDraw)
            RPC_Draw();
      }

      [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
      private void RPC_Win(Marks winnerMark)
      {
         if (_sessionDataModel.Mark == winnerMark)
            _gameEndUI.SetMenu("Win");
         else
            _gameEndUI.SetMenu("Lose");
      }

      [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
      private void RPC_Draw()
      {
         _gameEndUI.SetMenu("Draw");
      }
   }
}