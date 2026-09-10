using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridMap : MonoBehaviour
{
    private Tile[,] tiles;
    private int width;
    private int height;
    private float tileSize;

    private Vector3 origin;

    private MovementResolver movementResolver;
    private LightResolver lightResolver;

    [SerializeField]
    private LevelData levelData;

    private void Awake()
    {
        Initialize(levelData);
    }

    public void Initialize(LevelData levelData)
    {
        width = levelData.width;
        height = levelData.height;
        tileSize = levelData.tileSize;
        origin = levelData.origin;
        tiles = new Tile[width, height];

        movementResolver = new MovementResolver(this);
        lightResolver = new LightResolver(this);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 worldPosition = origin + new Vector3(x * tileSize, 0, y * tileSize);
                Vector2Int gridPos = new Vector2Int(x, y);

                tiles[x, y] = new Tile(gridPos, worldPosition);
            }
        }

        var sortedObjects = levelData.levelObjects.OrderBy(obj => obj.layer).ToList();

        foreach (var objData in sortedObjects)
        {
            var instance = Instantiate(objData.prefab, GridToWorld(objData.gridPosition), objData.orientation.ToRotation());
            instance.name = objData.objectId;
            var levelObj = instance.GetComponent<LevelObject>();

            if (levelObj == null)
            {
                Debug.LogError($"Prefab '{objData.prefab.name}' does not contain a LevelObject component.");

                Destroy(instance);
                continue;
            }

            levelObj.Initialize(objData, this);

            var tile = GetTile(levelObj.GridPosition);
            if (tile == null)
            {
                Debug.LogError($"Object '{objData.objectId}' has invalid grid position {objData.gridPosition}.");
                Destroy(instance);
                continue;
            }

            if (!tile.TryPlaceObject(objData.layer, levelObj))
            {
                Debug.LogError($"Could not place object '{objData.objectId}' on tile {objData.gridPosition}, layer {objData.layer}.");
                Destroy(instance);
                continue;
            }

        }
    }

    public Tile GetTile(Vector2Int gridPosition)
    {
        if (!IsValidPosition(gridPosition)) return null;
        return tiles[gridPosition.x, gridPosition.y];
    }

    public bool TryMoveToAdjacentTile(LevelObject obj, Direction direction)
    {
        return movementResolver != null && movementResolver.TryMoveToAdjacentTile(obj, direction);
    }

    public bool TryMoveContinuously(LevelObject obj, Vector3 targetWorldPosition)
    {
        return movementResolver != null && movementResolver.TryMoveContinuously(obj, targetWorldPosition);
    }

    public void ApplyIllumination(IEnumerable<Vector2Int> tiles, LevelObject source)
    {
        lightResolver.ApplyIllumination(tiles, source);
    }

    public void RemoveIllumination(IEnumerable<Vector2Int> tiles, LevelObject source)
    {
        lightResolver.RemoveIllumination(tiles, source);
    }

    public bool IsValidPosition(Vector2Int gridPosition)
    {
        return gridPosition.x >= 0 && gridPosition.x < width &&
               gridPosition.y >= 0 && gridPosition.y < height;
    }

    public Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        Vector3 relativePosition = worldPosition - origin;
        int x = Mathf.RoundToInt(relativePosition.x / tileSize);
        int y = Mathf.RoundToInt(relativePosition.z / tileSize);
        return new Vector2Int(x, y);
    }

    public Vector3 GridToWorld(Vector2Int gridPosition)
    {
        return origin + new Vector3(gridPosition.x * tileSize, 0, gridPosition.y * tileSize);
    }
}
