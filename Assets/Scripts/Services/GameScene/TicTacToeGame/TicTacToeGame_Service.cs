using System;
using Models;
using Services.GameScene.Input;
using Services.GameScene.NetworkGameLoop_Service;
using Services.ResourcesProvider;
using Services.TicTacToeGrid;
using StaticData;
using StaticData.Configs;
using StaticData.Enums;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Services.GameScene.TicTacToeGameController
{
   public class TicTacToeGame_Service : ITicTacToeGame_Service,
                                        IDisposable,
                                        IInitializable
   {
      private IInput_Service _inputService;
      private ITicTacToeGrid_Service _gridService;
      private TicTacToeGame_Model _ticTacToeGameModel;
      private GameLoop_Network _gameLoopNetwork;
      private GameLoopBridge_Network _gameLoopBridgeNetwork;
      private SessionData_Model _sessionDataModel;
      private Grid_Config _gridConfig;

      private CompositeDisposable _disposables = new CompositeDisposable();
      private CompositeDisposable _turnDisposables = new CompositeDisposable();
      private bool _yourTurn = false;

      [Inject]
      public void Construct(IInput_Service inputService,
                            ITicTacToeGrid_Service gridService,
                            TicTacToeGame_Model ticTacToeGameModel,
                            GameLoop_Network gameLoopNetwork,
                            GameLoopBridge_Network gameLoopBridgeNetwork,
                            SessionData_Model sessionDataModel,
                            IResourcesProvider_Service resourcesProviderService)
      {
         _inputService = inputService;
         _gridService = gridService;
         _ticTacToeGameModel = ticTacToeGameModel;
         _gameLoopNetwork = gameLoopNetwork;
         _gameLoopBridgeNetwork = gameLoopBridgeNetwork;
         _sessionDataModel = sessionDataModel;

         _gridConfig = resourcesProviderService.LoadResource<Grid_Config>(DataPaths_Record.GridConfig);
      }

      public void Initialize()
      {
         _disposables = new CompositeDisposable();
         _gameLoopNetwork.BeforeRematch.Subscribe(_ => Reset())
                         .AddTo(_disposables);
      }

      private void Reset()
      {
         _ticTacToeGameModel.Reset();
      }

      public void StartTurn()
      {
         _turnDisposables = new CompositeDisposable();
         _inputService.OnLeftClick
                      .Subscribe(OnLeftClick)
                      .AddTo(_turnDisposables);
         _yourTurn = true;
      }

      public void FinishTurn()
      {
         _turnDisposables.Dispose();
         _turnDisposables = null;
         _yourTurn = false;
      }
      
      private void OnLeftClick(Vector2 worldPosition)
      {
         // Debug.Log("left click");
         if (_yourTurn == false)
         {
            Debug.LogError("You've tried to handle left click on enemy turn. This shouldn't be possible");
            return;
         }

         Vector2Int? gridPosition = _gridService.WorldToGridPosition(worldPosition);
         // Debug.Log($"Clicked at grid position: {gridPosition}");

         if (gridPosition == null || _ticTacToeGameModel.GetMark(gridPosition.Value.x, gridPosition.Value.y) != Marks_Enum.None)
            return;

         _yourTurn = false;

         _gameLoopBridgeNetwork.RPC_RequestPlaceMark(gridPosition.Value.x, gridPosition.Value.y, _sessionDataModel.Mark);
         _gameLoopNetwork.RPC_PlayerFinishedTurn();
         _gameLoopNetwork.RPC_CheckWin();
      }

      public (bool isWin, Marks_Enum winnerMark, bool isDraw) CheckWin()
      {
         int winCondition = _gridConfig.MarksInRowToWin;
         int gridSize = _gridConfig.gridSize;
         bool hasEmptyCell = false;

         // Check all possible win conditions
         for (int x = 0; x < gridSize; x++)
         {
            for (int y = 0; y < gridSize; y++)
            {
               Marks_Enum currentMark = _ticTacToeGameModel.GetMark(x, y);
               if (currentMark == Marks_Enum.None)
               {
                  hasEmptyCell = true;
                  continue;
               }

               // Check horizontal win
               if (x <= gridSize - winCondition && CheckDirection(x, y, 1, 0, currentMark))
                  return (true, currentMark, false);

               // Check vertical win
               if (y <= gridSize - winCondition && CheckDirection(x, y, 0, 1, currentMark))
                  return (true, currentMark, false);

               // Check diagonal down-right win
               if (x <= gridSize - winCondition && y <= gridSize - winCondition &&
                   CheckDirection(x, y, 1, 1, currentMark))
                  return (true, currentMark, false);

               // Check diagonal down-left win
               if (x >= winCondition - 1 && y <= gridSize - winCondition &&
                   CheckDirection(x, y, -1, 1, currentMark))
                  return (true, currentMark, false);
            }
         }

         // If no win and no empty cells, it's a draw
         return (false, Marks_Enum.None, hasEmptyCell == false);

         bool CheckDirection(int startX,
                             int startY,
                             int deltaX,
                             int deltaY,
                             Marks_Enum mark)
         {
            for (int i = 0; i < winCondition; i++)
            {
               int x = startX + i * deltaX;
               int y = startY + i * deltaY;

               if (x < 0 || x >= gridSize || y < 0 || y >= gridSize)
                  return false;

               if (_ticTacToeGameModel.GetMark(x, y) != mark)
                  return false;
            }

            return true;
         }
      }


      public void Dispose()
      {
         _disposables.Dispose();
         _turnDisposables.Dispose();
      }
   }
}