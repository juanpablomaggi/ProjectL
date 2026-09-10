using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelObject : MonoBehaviour, ITileContent
{
    public string ObjectId { get; protected set; }
    public Vector2Int GridPosition { get; protected set; }
    public TileLayer Layer { get; protected set; }
    public Direction Orientation { get; protected set; }

    protected Action OnPlaceOnTile;
    protected Action OnRemoveFromTile;

    protected Tile currentTile;
    protected LevelObjectData data;
    protected Dictionary<Type, IObjectBehavior> behaviors = new();
    protected GridMap map;

    public Tile CurrentTile => currentTile;
    public GridMap Map => map;

    public virtual void Initialize(
        LevelObjectData objectData,
        GridMap gridMap)
    {
        data = objectData;
        ObjectId = objectData.objectId;
        GridPosition = objectData.gridPosition;
        Layer = objectData.layer;
        map = gridMap;

        Rotate(objectData.orientation);
        InitializeBehaviors();
    }

    protected virtual void InitializeBehaviors()
    {
        behaviors.Add(typeof(MovableComponent), new MovableComponent());
        behaviors.Add(typeof(InteractableComponent), new InteractableComponent());
        behaviors.Add(typeof(LightEmmiterComponent), new LightEmmiterComponent());
        behaviors.Add(typeof(LightBlockerComponent), new LightBlockerComponent());
        behaviors.Add(typeof(ColliderComponent), new ColliderComponent());
        behaviors.Add(typeof(StateComponent), new StateComponent());
        behaviors.Add(typeof(ActivableComponent), new ActivableComponent());

        ConfigureParameters(data.parameters);
    }

    public void ConfigureParameters(LevelObjectParameters parameters)
    {
        foreach (var behavior in behaviors.Values)
        {
            behavior.Configure(parameters);
        }
    }

    public T GetBehavior<T>() where T : IObjectBehavior
    {
        if (behaviors.TryGetValue(typeof(T), out var behavior))
        {
            return (T)behavior;
        }

        return default;
    }

    public virtual void Rotate(Direction newDirection)
    {
        Orientation = newDirection;
        transform.rotation = Orientation.ToRotation();
    }

    public virtual void PlaceOnTile(Tile tile)
    {
        if (tile == null) return;

        currentTile = tile;
        GridPosition = tile.GridPosition;
        OnPlaceOnTile?.Invoke();
        GetBehavior<LightEmmiterComponent>()?.ApplyIllumination(GridPosition, Orientation, Map);
    }

    public virtual void RemoveFromTile(Tile tile)
    {
        if (currentTile != tile) return;

        currentTile = null;
        OnRemoveFromTile?.Invoke();
        GetBehavior<LightEmmiterComponent>()?.RemoveIllumination(GridPosition, Orientation, Map);
    }
}