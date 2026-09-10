using UnityEngine;

public class Reflector : LevelObject
{
    public void Setup()
    {
        var interactable = GetBehavior<InteractableComponent>();
        interactable.SetupAction(Reflect);
    }

    public void Reflect(InteractionData data)
    {
        LightEmmiterComponent lightEmmiter = GetBehavior<LightEmmiterComponent>();

        lightEmmiter.RemoveIllumination(GridPosition, Orientation, Map);
        Direction newDirection = Orientation.RotateClockwise();
        Rotate(newDirection);
        lightEmmiter.ApplyIllumination(GridPosition, Orientation, Map);
    }

#if UNITY_EDITOR
    [ExecuteInEditMode]
    public void TestReflect()
    {
        Reflect(new InteractionData());
    }
#endif
}