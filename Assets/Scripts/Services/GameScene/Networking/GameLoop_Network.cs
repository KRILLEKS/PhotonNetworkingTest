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
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;
using Unit = UniRx.Unit;

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

      public Subject<Unit> BeforeRematch = new Subject<Unit>();
      public Subject<Unit> OnWin = new Subject<Unit>();
      public Subject<Unit> OnDraw = new Subject<Unit>();
      public Subject<Unit> OnLose = new Subject<Unit>();

      private GameLoop_StateMachine _gameLoopStateMachine;
      private SessionData_Model _sessionDataModel;
      private ITicTacToeGame_Service _ticTacToeGameService;
      private TurnIndicator_UI _turnIndicatorUI;

      [Networked] private int CurrentTurnIndex { get; set; }
      private List<PlayerRef> TurnOrder { get; set; } // only host has this info
      private List<PlayerRef> RematchVotes { get; set; }


      [Inject]
      private void Construct(GameLoop_StateMachine gameLoopStateMachine,
                             SessionData_Model sessionDataModel,
                             ITicTacToeGame_Service ticTacToeGameService,
                             TurnIndicator_UI turnIndicatorUI)
      {
         _gameLoopStateMachine = gameLoopStateMachine;
         _sessionDataModel = sessionDataModel;
         _ticTacToeGameService = ticTacToeGameService;
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
         if (Object.HasStateAuthority == false)
            return;

         RematchVotes = new List<PlayerRef>();

         // Get all players and create random turn order
         var players = new List<PlayerRef>(Runner.ActivePlayers);
         players.Shuffle(); // Randomize turn order

         TurnOrder = new List<PlayerRef>();
         foreach (var player in players)
            TurnOrder.Add(player);

         if (TurnOrder.Count == 2)
         {
            RPC_SetSessionData(TurnOrder[0], Marks_Enum.Cross);
            RPC_SetSessionData(TurnOrder[1], Marks_Enum.Circle);
         }
         // for testing only
         else
            foreach (var player in TurnOrder)
               RPC_SetSessionData(player, Marks_Enum.Cross);

         // Set initial turn to first player
         CurrentTurnIndex = 0;
      }

      public void StartGame()
      {
         if (Object.HasStateAuthority == false)
            return;

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
      private void RPC_SetSessionData([RpcTarget] PlayerRef playerRef, Marks_Enum mark)
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
         (bool isWin, Marks_Enum winnerMark, bool isDraw) winInfo = _ticTacToeGameService.CheckWin();
         if (winInfo.isWin)
            RPC_WinLose(winInfo.winnerMark);
         else if (winInfo.isDraw)
            RPC_Draw();
      }

      [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
      private void RPC_WinLose(Marks_Enum winnerMark)
      {
         if (_sessionDataModel.Mark == winnerMark)
            OnWin?.OnNext(Unit.Default);
         else
            OnLose?.OnNext(Unit.Default);
      }

      [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
      private void RPC_Draw()
      {
         OnDraw?.OnNext(Unit.Default);
      }

      [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
      public void RPC_VoteForRematch(RpcInfo rpcInfo = default)
      {
         PlayerRef playerRef = rpcInfo.Source;
         if (RematchVotes.Contains(playerRef))
            return;

         RematchVotes.Add(playerRef);
         Debug.Log("Votes to rematch: " + RematchVotes.Count);

         // all players vote for rematch
         if (RematchVotes.Count == TurnOrder.Count)
            RPC_Rematch();
      }

      [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
      private void RPC_Rematch()
      {
         BeforeRematch?.OnNext(Unit.Default);
         Debug.Log("Rematch!");
         InitializeGame();
         StartGame();
      }
   }
}