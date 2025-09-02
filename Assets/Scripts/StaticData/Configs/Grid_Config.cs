using UnityEngine;

namespace StaticData.Configs
{
   [CreateAssetMenu(menuName = "Configs/Grid", fileName = "GridConfig")]
   public class Grid_Config : ScriptableObject
   {
      [Header("Grid Settings")]
      public int gridSize = 3; // 3x3 grid for Tic Tac Toe
      public float edgeOffset = 0.1f; // Offset from the edges in world units
      public int MarksInRowToWin = 3;

      [Header("Line Renderer Settings")]
      public Material lineMaterial;
      public float lineWidth = 0.05f;
      public Color lineColor = Color.white;

      [Header("Mark Settings")]
      [Range(0, 0.8f)] public float markPadding = 0.5f;
   }
}