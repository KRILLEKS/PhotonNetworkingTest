using Services.ResourcesProvider;
using Services.TicTacToeGrid;
using StaticData;
using StaticData.Enums;
using UnityEngine;
using Zenject;

namespace Services.GameScene.PrefabFactory
{
   public class GamePrefabFactory_Service : IGamePrefabFactory_Service
   {
      private IResourcesProvider_Service _resourcesProviderService;
      private ITicTacToeGrid_Service _ticTacToeGridService;
      private float _markPadding = 0.5f; // TODO: move it to the correct location

      [Inject]
      private void Construct(IResourcesProvider_Service resourcesProviderService, ITicTacToeGrid_Service ticTacToeGridService)
      {
         _resourcesProviderService = resourcesProviderService;
         _ticTacToeGridService = ticTacToeGridService;
      }

      public GameObject SpawnMark(Marks mark, Vector3 position, Transform parent)
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
               case Marks.None:
                  Debug.LogError("Cannot spawn Mark.None");
                  return null;
               case Marks.Circle:
                  return DataPaths_Record.CirclePrefab;
               case Marks.Cross:
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
            float targetSize = cellSize * (1f - _markPadding);
            float scale = targetSize / maxDimension;

            markObject.transform.localScale = new Vector3(scale, scale, 1f);
         }
      }
   }
}