using StaticData.Enums;
using UniRx;
using UnityEngine;

namespace Models
{
   public class TicTacToeGame_Model
   {
      public Subject<(Vector2Int, Marks)> OnMarkChange = new Subject<(Vector2Int, Marks)>();

      private Marks[,] marks;

      public void Initialize(int gridSize)
      {
         var marksArray = new Marks[gridSize, gridSize];
         for (int x = 0; x < gridSize; x++)
            for (int y = 0; y < gridSize; y++)
               marksArray[x, y] = Marks.None;

         marks = marksArray;
      }

      public void SetMark(int x, int y, Marks mark)
      {
         if (marks == null)
         {
            Debug.LogError("Mark are not initialized");
            return;
         }

         if (marks[x, y] != Marks.None)
            return;

         marks[x, y] = mark;
         OnMarkChange.OnNext((new Vector2Int(x, y), mark));
      }

      public Marks GetMark(int x, int y)
      {
         return marks[x, y];
      }
   }
}