using UnityEngine;

public class MovementResolver
{
    private readonly GridMap map;

    public MovementResolver(GridMap map)
    {
        this.map = map;
    }

    public bool TryMoveToAdjacentTile(LevelObject obj, Direction direction)
    {
        if (!TryGetMovementContext(obj, out var movable, out var originTile))
            return false;

        Tile targetTile = map.GetTile(originTile.GridPosition + direction.ToVector2Int());
        if (targetTile == null)
            return false;

        var movementData = new MovementData(obj, originTile, targetTile, targetTile.WorldPosition);
        return TryExecuteMovement(movable, movementData);
    }

    public bool TryMoveContinuously(LevelObject obj, Vector3 targetWorldPosition)
    {
        if (!TryGetMovementContext(obj, out var movable, out var originTile))
            return false;

        if (movable.MovableType != MovableType.CONTINUOUS)
            return false;

        Tile targetTile = map.GetTile(map.WorldToGrid(targetWorldPosition));
        if (targetTile == null)
            return false;

        var movementData = new MovementData(obj, originTile, targetTile, targetWorldPosition);
        return TryExecuteMovement(movable, movementData);
    }

    private bool TryGetMovementContext(LevelObject obj, out MovableComponent movable, out Tile originTile)
    {
        movable = null;
        originTile = null;

        if (obj == null) return false;

        movable = obj.GetBehavior<MovableComponent>();
        originTile = obj.CurrentTile;

        return movable != null &&
               movable.IsMovable &&
               !movable.IsMoving &&
               originTile != null;
    }

    private bool TryExecuteMovement(MovableComponent movable, MovementData data)
    {
        if (!movable.CanMove(data)) return false;

        if (data.ChangesTile && !CanEnterTile(data, movable.MovementType)) return false;

        if (data.ChangesTile && !ChangeTile(data)) return false;

        movable.Move(data);
        return true;
    }

    private bool CanEnterTile(MovementData data, MovementType movementType)
    {
        return data.TargetTile.IsEmpty(data.Object.Layer) &&
               !data.TargetTile.BlocksMovement(movementType, data.Object);
    }

    private bool ChangeTile(MovementData data)
    {
        if (!data.OriginTile.RemoveObject(data.Object)) return false;

        if (!data.TargetTile.TryPlaceObject(data.Object.Layer, data.Object))
        {
            data.OriginTile.TryPlaceObject(data.Object.Layer, data.Object);
            return false;
        }

        return true;
    }
}
