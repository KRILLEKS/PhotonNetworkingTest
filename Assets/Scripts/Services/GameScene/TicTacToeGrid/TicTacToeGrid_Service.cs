using System.Collections;
using System.Collections.Generic;
using Models;
using Services.ResourcesProvider;
using StaticData;
using StaticData.Configs;
using UnityEngine;
using Zenject;

namespace Services.TicTacToeGrid
{
   public class TicTacToeGrid_Service : ITicTacToeGrid_Service
   {
      private Grid_Config _gridConfig;
      private TicTacToeGame_Model _ticTacToeGameModel;

      private GameObject _contentParent;
      private LineRenderer[] gridLines;
      private float cellSize;
      private float totalGridSize;

      [Inject]
      private void Construct(IResourcesProvider_Service resourcesProviderService, TicTacToeGame_Model ticTacToeGameModel)
      {
         _gridConfig = resourcesProviderService.LoadResource<Grid_Config>(DataPaths_Record.GridConfig);
         _ticTacToeGameModel = ticTacToeGameModel;
      }

      public void GenerateGrid()
      {
         _contentParent = new GameObject("Grid");
         _contentParent.transform.position = Vector3.zero;

         // Clear existing grid if any
         ClearGrid();

         // Calculate maximum possible grid size based on camera view
         CalculateGridSize();

         // Create line renderers for grid lines
         // For 3x3 grid, we need 4 vertical and 4 horizontal lines = 8 total
         gridLines = new LineRenderer[(_gridConfig.gridSize + 1) * 2];

         CreateGridLines();
         
         _ticTacToeGameModel.Initialize(_gridConfig.gridSize);
      }

      private void CalculateGridSize()
      {
         // Get camera reference (using main camera by default)
         Camera mainCamera = Camera.main;

         if (mainCamera == null)
         {
            Debug.LogWarning("Main camera not found. Using default cell size.");
            cellSize = 1f;
            totalGridSize = _gridConfig.gridSize * cellSize;
            return;
         }

         // Calculate available space in world units
         float cameraHeight = 2f * mainCamera.orthographicSize;
         float cameraWidth = cameraHeight * mainCamera.aspect;

         // Subtract edge offsets from both sides
         float availableWidth = cameraWidth - (2f * _gridConfig.edgeOffset);
         float availableHeight = cameraHeight - (2f * _gridConfig.edgeOffset);

         // Use the smaller dimension to ensure grid fits both ways
         float maxGridDimension = Mathf.Min(availableWidth, availableHeight);

         // Calculate cell size based on grid size
         cellSize = maxGridDimension / _gridConfig.gridSize;
         totalGridSize = _gridConfig.gridSize * cellSize;

         Debug.Log($"Grid calculated: CellSize={cellSize}, TotalSize={totalGridSize}");
      }

      private void CreateGridLines()
      {
         int lineIndex = 0;

         // Create vertical lines
         for (int i = 0; i <= _gridConfig.gridSize; i++)
         {
            GameObject lineObject = new GameObject($"VerticalLine_{i}");
            lineObject.transform.SetParent(_contentParent.transform);
            lineObject.transform.localPosition = Vector3.zero;

            LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
            SetupLineRenderer(lineRenderer);

            // Calculate line positions
            float xPos = (i * cellSize) - (totalGridSize / 2f);
            Vector3 startPos = new Vector3(xPos, -totalGridSize / 2f, 0f);
            Vector3 endPos = new Vector3(xPos, totalGridSize / 2f, 0f);

            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, endPos);

            gridLines[lineIndex++] = lineRenderer;
         }

         // Create horizontal lines
         for (int i = 0; i <= _gridConfig.gridSize; i++)
         {
            GameObject lineObject = new GameObject($"HorizontalLine_{i}");
            lineObject.transform.SetParent(_contentParent.transform);
            lineObject.transform.localPosition = Vector3.zero;

            LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
            SetupLineRenderer(lineRenderer);

            // Calculate line positions
            float yPos = (i * cellSize) - (totalGridSize / 2f);
            Vector3 startPos = new Vector3(-totalGridSize / 2f, yPos, 0f);
            Vector3 endPos = new Vector3(totalGridSize / 2f, yPos, 0f);

            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, endPos);

            gridLines[lineIndex++] = lineRenderer;
         }
      }

      private void SetupLineRenderer(LineRenderer lineRenderer)
      {
         lineRenderer.material = _gridConfig.lineMaterial;
         lineRenderer.startColor = _gridConfig.lineColor;
         lineRenderer.endColor = _gridConfig.lineColor;
         lineRenderer.startWidth = _gridConfig.lineWidth;
         lineRenderer.endWidth = _gridConfig.lineWidth;
         lineRenderer.positionCount = 2;
         lineRenderer.useWorldSpace = false; // Use local space for easier positioning
      }

      public void ClearGrid()
      {
         if (gridLines == null)
            return;

         foreach (LineRenderer line in gridLines)
            if (line != null && line.gameObject != null)
               Object.DestroyImmediate(line.gameObject);

         // Also destroy any existing child objects
         foreach (Transform child in _contentParent.transform)
            Object.DestroyImmediate(child.gameObject);

         gridLines = null;
      }

      public Vector2Int? WorldToGridPosition(Vector3 worldPosition)
      {
         Vector3 localPos = _contentParent.transform.InverseTransformPoint(worldPosition);

         // Convert to grid coordinates (0,0 is bottom-left)
         localPos += new Vector3(totalGridSize / 2f, totalGridSize / 2f, 0f);

         int x = Mathf.FloorToInt(localPos.x / cellSize);
         int y = Mathf.FloorToInt(localPos.y / cellSize);

         if (x < 0 || x >= _gridConfig.gridSize ||
             y < 0 || y >= _gridConfig.gridSize)
            return null;

         return new Vector2Int(x, y);
      }

      public Vector3 GridToWorldPosition(int x, int y)
      {
         float posX = (x * cellSize) - (totalGridSize / 2f) + (cellSize / 2f);
         float posY = (y * cellSize) - (totalGridSize / 2f) + (cellSize / 2f);

         return _contentParent.transform.TransformPoint(new Vector3(posX, posY, 0f));
      }

      public float GetCellSize()
      {
         return cellSize;
      }

      public float GetTotalGridSize()
      {
         return totalGridSize;
      }
   }
}