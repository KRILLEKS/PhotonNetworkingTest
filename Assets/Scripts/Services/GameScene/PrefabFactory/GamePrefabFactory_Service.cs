using Services.ResourcesProvider;
using Services.TicTacToeGrid;
using StaticData;
using StaticData.Configs;
using StaticData.Enums;
using UnityEngine;
using Zenject;

namespace Services.GameScene.PrefabFactory
{
   public class GamePrefabFactory_Service : IGamePrefabFactory_Service, IInitializable
   {
      private IResourcesProvider_Service _resourcesProviderService;
      private ITicTacToeGrid_Service _ticTacToeGridService;
      private Grid_Config _gridConfig;

      [Inject]
      private void Construct(IResourcesProvider_Service resourcesProviderService, ITicTacToeGrid_Service ticTacToeGridService)
      {
         _resourcesProviderService = resourcesProviderService;
         _ticTacToeGridService = ticTacToeGridService;
      }
      public void Initialize()
      {
         _gridConfig = _resourcesProviderService.LoadResource<Grid_Config>(DataPaths_Record.GridConfig);
      }

      public GameObject SpawnMark(Marks_Enum mark, Vector3 position, Transform parent)
      {
         string path = GetPath();
         if (path == null)
         {
            Debug.LogError("No path for mark: " + mark);
            return new GameObject("Empty mark");
         }

         GameObject prefab = _resourcesProviderService.LoadResource<GameObject>(path);
         var instance = Object.Instantiate(prefab, position, Quaternion.identity, parent);

         // Scale the mark to fit the cell
         ScaleToFitCell(instance, _ticTacToeGridService.GetCellSize());

         return instance;

         string GetPath()
         {
            switch (mark)
            {
               case Marks_Enum.None:
                  Debug.LogError("Cannot spawn Mark.None");
                  return null;
               case Marks_Enum.Circle:
                  return DataPaths_Record.CirclePrefab;
               case Marks_Enum.Cross:
                  return DataPaths_Record.CrossPrefab;
               default:
                  Debug.LogError($"Undefined mark type: {mark}");
                  return null;
            }
         }
         void ScaleToFitCell(GameObject markObject, float cellSize)
         {
            SpriteRenderer spriteRenderer = markObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
               Debug.LogWarning("Mark prefab doesn't have a SpriteRenderer");
               return;
            }

            Bounds spriteBounds = spriteRenderer.sprite.bounds;
            float maxDimension = Mathf.Max(spriteBounds.size.x, spriteBounds.size.y);

            // Calculate scale to fit within the cell with padding
            float targetSize = cellSize * (1f - _gridConfig.markPadding);
            float scale = targetSize / maxDimension;

            markObject.transform.localScale = new Vector3(scale, scale, 1f);
         }
      }

   }
}