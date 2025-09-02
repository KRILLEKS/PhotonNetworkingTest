using StaticData.Enums;
using UniRx;
using UnityEngine;

namespace Models
{
   public class SessionData_Model
   {
      public Subject<Marks_Enum> OnMarkChange = new Subject<Marks_Enum>();

      public Color Color;

      public Marks_Enum Mark
      {
         get
         {
            return _mark;
         }
         set
         {
            _mark = value;
            OnMarkChange.OnNext(_mark);
         }
      }

      private Marks_Enum _mark;
   }
}