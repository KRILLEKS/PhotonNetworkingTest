using System;
using Models;
using Services.GameScene.Input;
using Services.GameScene.NetworkGameLoop_Service;
using Services.TicTacToeGrid;
using StaticData.Enums;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Services.GameScene.TicTacToeGameController
{
   public class TicTacToeGame_Service : ITicTacToeGame_Service,
                                        IDisposable
   {
      private IInput_Service _inputService;
      private ITicTacToeGrid_Service _gridService;
      private TicTacToeGame_Model _ticTacToeGameModel;
      private GameLoop_Network _gameLoopNetwork;
      private SessionData_Model _sessionDataModel;

      private CompositeDisposable _disposables = new CompositeDisposable();
      private bool _yourTurn = false;

      [Inject]
      public void Construct(IInput_Service inputService,
                            ITicTacToeGrid_Service gridService,
                            TicTacToeGame_Model ticTacToeGameModel,
                            GameLoop_Network gameLoopNetwork,
                            SessionData_Model sessionDataModel)
      {
         _inputService = inputService;
         _gridService = gridService;
         _ticTacToeGameModel = ticTacToeGameModel;
         _gameLoopNetwork = gameLoopNetwork;
         _sessionDataModel = sessionDataModel;
      }

      public void StartTurn()
      {
         _disposables = new CompositeDisposable();
         _inputService.OnLeftClick
                      .Subscribe(OnLeftClick)
                      .AddTo(_disposables);
         _yourTurn = true;
      }

      public void FinishTurn()
      {
         _disposables.Dispose();
         _disposables = null;
         _yourTurn = false;
      }

      private void OnLeftClick(Vector2 worldPosition)
      {
         // Debug.Log("left click");
         if (_yourTurn == false)
            return;
         
         Vector2Int? gridPosition = _gridService.WorldToGridPosition(worldPosition);
         // Debug.Log($"Clicked at grid position: {gridPosition}");

         // out of grid borders
         if (gridPosition == null)
            return;

         _yourTurn = false;

         // Handle the game logic here (place X/O, check for win, etc.)
         _gameLoopNetwork.RPC_PlayerFinishedTurn();
         _gameLoopNetwork.RPC_RequestPlaceMark(gridPosition.Value.x, gridPosition.Value.y, _sessionDataModel.Marks);
      }

      public void Dispose()
      {
      }
   }
}