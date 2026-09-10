using System;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public Vector2Int GridPosition { get; private set; }
    public Vector3 WorldPosition { get; private set; }

    public event Action<Tile, bool> OnIlluminationChanged;
    public event Action<LevelObject, TileLayer> OnObjectEnter;
    public event Action<LevelObject, TileLayer> OnObjectExit;

    public bool IsIlluminated => lightEmitters.Count > 0;

    private readonly Dictionary<TileLayer, LevelObject> contents;
    private readonly HashSet<LevelObject> lightEmitters;

    public Tile(Vector2Int gridPos, Vector3 worldPos)
    {
        GridPosition = gridPos;
        WorldPosition = worldPos;
        lightEmitters = new HashSet<LevelObject>();
        contents = new Dictionary<TileLayer, LevelObject>();
    }

    public bool TryPlaceObject(TileLayer layer, LevelObject obj)
    {
        if (obj == null || !IsEmpty(layer)) return false;

        contents[layer] = obj;
        obj.PlaceOnTile(this);
        OnObjectEnter?.Invoke(obj, layer);
        return true;
    }

    public bool RemoveObject(LevelObject obj)
    {
        if (obj == null) return false;

        if (!contents.TryGetValue(obj.Layer, out var currentObject)) return false;

        if (currentObject != obj) return false;

        contents.Remove(obj.Layer);
        obj.RemoveFromTile(this);
        OnObjectExit?.Invoke(obj, obj.Layer);

        return true;
    }

    public bool RemoveObject(TileLayer layer)
    {
        if (!contents.TryGetValue(layer, out var obj)) return false;

        contents.Remove(layer);
        obj.RemoveFromTile(this);
        OnObjectExit?.Invoke(obj, layer);

        return true;
    }

    public LevelObject GetObject(TileLayer layer)
    {
        contents.TryGetValue(layer, out var obj);
        return obj;
    }

    public bool IsEmpty(TileLayer layer)
    {
        return !contents.TryGetValue(layer, out var obj) || obj == null;
    }

    public bool BlocksMovement(MovementType movementType, LevelObject movingObject)
    {
        if (IsEmpty(TileLayer.BASE_TILE)) return true;

        foreach (var obj in contents.Values)
        {
            if (obj == null || obj == movingObject)
                continue;

            var collider = obj.GetBehavior<ColliderComponent>();
            if (collider != null && collider.Blocks(movementType))
                return true;
        }

        return false;
    }

    public void AddLightSource(LevelObject source)
    {
        if (lightEmitters.Add(source) && lightEmitters.Count == 1)
        {
            OnIlluminationChanged?.Invoke(this, true);
        }
    }

    public void RemoveLightSource(LevelObject source)
    {
        if (lightEmitters.Remove(source) && lightEmitters.Count == 0)
        {
            OnIlluminationChanged?.Invoke(this, false);
        }
    }
}
