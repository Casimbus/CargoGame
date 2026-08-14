using System;
using UnityEngine;
using UnityEngine.UI;

    //MULTIPLIER FOR COSTS
public enum TerrainType
{
    PLAIN,
    WATER,
    MOUNTAIN
}
//ORIGINAL COSTS
public enum BuildingType
{
    ROAD,
    RAIL,
    CANAL,
}

public class TileManager : MonoBehaviour
{
    [Header("Tile Info")]
    [SerializeField] private TerrainType terrainType;
    [SerializeField] private BuildingType buildingType;
    [SerializeField] private Image image;
    [SerializeField] private int[] terrainCosts;
    [SerializeField] private int[] buildingCosts;
    
    private int cost;
    public TerrainType TerrainType => terrainType;
    public BuildingType BuildingType => buildingType;
    
    public void SetBuilding(BuildingType type)
    {
        buildingType = type;
    }

    private void CalculateCosts()
    {
        int buldingCost = buildingCosts[(int)buildingType];
        cost = terrainCosts[(int)terrainType] * buldingCost; //MULTIPLIER
    }
}
