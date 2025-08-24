using System;
using UnityEngine;

namespace Services.GameScene.Input
{
   public interface IInput_Service
   {
      IObservable<Vector2> OnLeftClick
      {
         get;
      }
   }
}