using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    [SerializeField] private RectTransform gameArea;
   [SerializeField] private Image tile;
   [SerializeField] private float tileSize;   
    void Start()
    {
        GenerateGrid();
    }


    void Update()
    {
        
    }

    private void GenerateGrid()
    {
        int columns = Mathf.CeilToInt(gameArea.rect.width / tileSize);
        int rows = Mathf.CeilToInt(gameArea.rect.height / tileSize);
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Image newTile = Instantiate(tile, transform);
              RectTransform rect = newTile.rectTransform;
              rect.sizeDelta = new Vector2(tileSize, tileSize);
              rect.anchorMin = Vector2.zero;
              rect.anchorMax = Vector2.zero;
              rect.pivot = new Vector2(0.5f, 0.5f);
              rect.anchoredPosition = new Vector2((x * tileSize + tileSize / 2f) - gameArea.rect.width / 2f,(y * tileSize + tileSize / 2) - gameArea.rect.height / 2f);
              Debug.Log(newTile.name + " Pos: " + newTile.rectTransform.anchoredPosition);
              Debug.Log(" Rows: " + rows + " Columns: " + columns);
            }
        }
    }

}
