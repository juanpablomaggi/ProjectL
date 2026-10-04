using UnityEngine;

[System.Serializable]
public class LevelObjectData
{
    [Header("General")]
    public string objectId;
    public GameObject prefab;

    [Header("Grid Settings")]
    public Vector2Int gridPosition;
    public TileLayer layer;

    [Header("Orientation")]
    public Direction orientation;

    [Header("Parameters")]
    public LevelObjectParameters parameters;

    public LevelObjectData Clone()
    {
        LevelObjectData newData = new LevelObjectData();
        newData.objectId = objectId;
        newData.prefab = prefab;

        newData.gridPosition = gridPosition;
        newData.layer = layer;
        
        newData.orientation = orientation;
        newData.parameters = parameters.Clone();
        return newData;
    }
}