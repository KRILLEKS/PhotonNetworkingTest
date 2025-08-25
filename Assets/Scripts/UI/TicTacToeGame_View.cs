using System;
using Models;
using Services.GameScene.PrefabFactory;
using Services.TicTacToeGrid;
using StaticData.Enums;
using UniRx;
using UnityEngine;
using Zenject;

namespace UI
{
   public class TicTacToeGame_View : MonoBehaviour
   {
      private TicTacToeGame_Model _ticTacToeGameModel;
      private IGamePrefabFactory_Service _gamePrefabFactoryService;
      private ITicTacToeGrid_Service _ticTacToeGridService;

      private GameObject _content;

      [Inject]
      private void Construct(TicTacToeGame_Model ticTacToeGameModel, IGamePrefabFactory_Service gamePrefabFactoryService, ITicTacToeGrid_Service ticTacToeGridService)
      {
         _ticTacToeGameModel = ticTacToeGameModel;
         _gamePrefabFactoryService = gamePrefabFactoryService;
         _ticTacToeGridService = ticTacToeGridService;
      }

      private void Start()
      {
         
         _ticTacToeGameModel.OnMarkChange.Subscribe(changeEvent =>
                            {
                               Vector2Int position = changeEvent.Item1;
                               Marks mark = changeEvent.Item2;

                               OnMarkChange(position, mark);
                            })
                            .AddTo(this); // Auto unsubscribe when destroyed 

         _content = new GameObject("Marks");
      }
      

      private void OnMarkChange(Vector2Int position, Marks mark)
      {
         Debug.Log($"Mark changed at {position} to {mark}");
         Vector3 worldPosition = _ticTacToeGridService.GridToWorldPosition(position.x, position.y);
         _gamePrefabFactoryService.SpawnMark(mark, worldPosition, _content.transform);
      }
   }
}