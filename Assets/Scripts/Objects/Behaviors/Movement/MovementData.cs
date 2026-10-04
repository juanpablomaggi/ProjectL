using System.Collections.Generic;
using UnityEngine;

public struct MovementData
{
    public LevelObject Object { get; }
    public Tile OriginTile { get; }
    public Tile TargetTile { get; }
    public Vector3 TargetWorldPosition { get; }
    public bool ChangesTile => OriginTile != TargetTile;
    public IEnumerable<LevelObject> LinkedObjects { get; }

    public MovementData(
        LevelObject obj,
        Tile originTile,
        Tile targetTile,
        Vector3 targetWorldPosition,
        IEnumerable<LevelObject> linkedObjects = null)
    {
        Object = obj;
        OriginTile = originTile;
        TargetTile = targetTile;
        TargetWorldPosition = targetWorldPosition;
        LinkedObjects = linkedObjects;
    }
}
