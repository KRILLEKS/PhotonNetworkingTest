using System;
using Models;
using Services.GameScene.Input;
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

      private CompositeDisposable _disposables = new CompositeDisposable();

      [Inject]
      public void Construct(IInput_Service inputService,
                            ITicTacToeGrid_Service gridService,
                            TicTacToeGame_Model ticTacToeGameModel)
      {
         _inputService = inputService;
         _gridService = gridService;
         _ticTacToeGameModel = ticTacToeGameModel;
      }

      public void StartTurn()
      {
            _inputService.OnLeftClick
                         .Subscribe(OnLeftClick)
                         .AddTo(_disposables);
      }

      public void FinishTurn()
      {
         
            _disposables.Dispose();
      }

      private void OnLeftClick(Vector2 worldPosition)
      {
         Vector2Int? gridPosition = _gridService.WorldToGridPosition(worldPosition);
         // Debug.Log($"Clicked at grid position: {gridPosition}");

         // out of grid borders
         if (gridPosition == null)
            return;

         // Handle the game logic here (place X/O, check for win, etc.)
         _ticTacToeGameModel.SetMark(gridPosition.Value.x, gridPosition.Value.y, (Marks)Random.Range(1, 3));
      }

      public void Dispose()
      {
      }
   }
}