using UnityEngine;

namespace Services.TicTacToeGrid
{
   public interface ITicTacToeGrid_Service
   {
      void GenerateGrid();
      Vector2Int? WorldToGridPosition(Vector3 worldPosition);
      Vector3 GridToWorldPosition(int x, int y);
      float GetCellSize();
   }
}