using UnityEngine;

public class LightBlockerComponent : IObjectBehavior
{
    private ILightBlocker behavior;
    private ILightBlocker carriedBehavior;

    public void Configure(LevelObjectParameters data)
    {
        behavior = data.lightBlockType == LightBlockType.OPAQUE ? new OpaqueBehavior() : new TransparentBehavior();
    }

    public void CopyFrom(LightBlockerComponent source)
    {
        carriedBehavior = source?.behavior;
    }

    public void ClearCopiedBehavior()
    {
        carriedBehavior = null;
    }

    public bool BlocksLight(Vector2Int origin, Vector2Int target)
    {
        return (carriedBehavior ?? behavior).BlocksLight(origin, target);
    }
}

public enum LightBlockType
{
    OPAQUE,
    TRANSPARENT
}
