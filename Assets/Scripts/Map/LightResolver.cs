using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class LightResolver
{
    private readonly GridMap map;

    public LightResolver(GridMap map)
    {
        this.map = map;
    }

    public void ApplyIllumination(IEnumerable<Vector2Int> tiles, LevelObject source)
    {
        foreach (var pos in tiles)
        {
            var tile = map.GetTile(pos);
            if (tile != null)
                tile.AddLightSource(source);
        }
    }

    public void RemoveIllumination(IEnumerable<Vector2Int> tiles, LevelObject source)
    {
        foreach (var pos in tiles)
        {
            var tile = map.GetTile(pos);
            if (tile != null)
                tile.RemoveLightSource(source);
        }
    }
}
