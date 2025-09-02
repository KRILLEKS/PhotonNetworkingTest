using StaticData.Enums;
using UniRx;
using UnityEngine;

namespace Models
{
   public class TicTacToeGame_Model
   {
      public Subject<(Vector2Int, Marks_Enum)> OnMarkChange = new Subject<(Vector2Int, Marks_Enum)>();
      public Subject<Unit> OnReset = new Subject<Unit>();

      private Marks_Enum[,] marks;

      public void Reset()
      {
         Initialize(marks.GetLength(0));
         OnReset?.OnNext(Unit.Default);
      }
      
      public void Initialize(int gridSize)
      {
         var marksArray = new Marks_Enum[gridSize, gridSize];
         for (int x = 0; x < gridSize; x++)
            for (int y = 0; y < gridSize; y++)
               marksArray[x, y] = Marks_Enum.None;

         marks = marksArray;
      }

      public void SetMark(int x, int y, Marks_Enum mark)
      {
         if (marks == null)
         {
            Debug.LogError("Mark are not initialized");
            return;
         }

         if (marks[x, y] != Marks_Enum.None)
            return;

         marks[x, y] = mark;
         OnMarkChange.OnNext((new Vector2Int(x, y), mark));
      }

      public Marks_Enum GetMark(int x, int y)
      {
         return marks[x, y];
      }
   }
}