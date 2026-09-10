using System.Numerics;
using UnityEngine;

public class LightEmmiterComponent : IObjectBehavior
{
    private ILightEmitterBehavior emitter;
    private int range;

    public void Configure(LevelObjectParameters data)
    {
        range = data.lightRange;
        switch (data.lightShape)
        {
            case LightShape.CONE:
                emitter = new ConeLightEmmiterBehavior(range);
                break;
            case LightShape.AREA:
                emitter = new SquareLightEmmiterBehavior(range);
                break;
            case LightShape.LINE:
                emitter = new LineLightEmmiterBehavior(range);
                break;
            case LightShape.NONE:
            default:
                emitter = new NoLightEmmiterBehavior();
                break;
        }
    }

    public void ApplyIllumination(Vector2Int gridPosition, Direction orientation, GridMap map)
    {
        var tiles = emitter.GetIlluminatedTiles(gridPosition, orientation);
        map.ApplyIllumination(tiles, null);
    }

    public void RemoveIllumination(Vector2Int gridPosition, Direction orientation, GridMap map)
    {
        var tiles = emitter.GetIlluminatedTiles(gridPosition, orientation);
        map.RemoveIllumination(tiles, null);
    }
}

public enum LightShape
{
    NONE,
    CONE,
    AREA,
    LINE
}
