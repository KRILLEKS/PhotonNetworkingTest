using StaticData.Enums;
using UnityEngine;

namespace Services.GameScene.PrefabFactory
{
   public interface IGamePrefabFactory_Service
   {
      GameObject SpawnMark(Marks_Enum mark, Vector3 position, Transform parent);
   }
}