using UnityEngine;

public class Reflector : LevelObject
{
    protected override void InitializeBehaviors()
    {
        base.InitializeBehaviors();
        var interactable = GetBehavior<InteractableComponent>();
        interactable.SetupAction(Reflect);
    }

    public void Reflect(InteractionData data)
    {
        LightEmmiterComponent lightEmmiter = GetBehavior<LightEmmiterComponent>();

        lightEmmiter.RemoveIllumination(GridPosition, Orientation, Map, this);
        Direction newDirection = Orientation.RotateClockwise();
        Rotate(newDirection);
        lightEmmiter.ApplyIllumination(GridPosition, Orientation, Map, this);
    }

#if UNITY_EDITOR
    [ExecuteInEditMode]
    public void TestReflect()
    {
        Reflect(new InteractionData());
    }
#endif
}
