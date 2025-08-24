using System;
using Services.GameScene.Input;
using Services.TicTacToeGrid;
using UniRx;
using UnityEngine;
using Zenject;

namespace Services.GameScene.TicTacToeGameController
{
   public class TicTacToeGameController_Service : ITicTacToeGameController_Service, IInitializable, IDisposable
   {
      private IInput_Service _inputService;
      private ITicTacToeGrid_Service _gridService;

      private CompositeDisposable _disposables = new CompositeDisposable();

      [Inject]
      public void Construct(IInput_Service inputService,
                            ITicTacToeGrid_Service gridService)
      {
         _inputService = inputService;
         _gridService = gridService;
      }

      public void Initialize()
      {
         _inputService.OnLeftClick
                      .Subscribe(OnLeftClick)
                      .AddTo(_disposables);
      }

      private void OnLeftClick(Vector2 worldPosition)
      {
         Vector2Int? gridPosition = _gridService.WorldToGridPosition(worldPosition);

         Debug.Log($"Clicked at grid position: {gridPosition}");

         // Handle the game logic here (place X/O, check for win, etc.)
      }

      public void Dispose()
      {
         _disposables.Dispose();
      }
   }
}