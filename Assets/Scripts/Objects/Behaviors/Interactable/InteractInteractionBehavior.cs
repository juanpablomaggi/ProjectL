using System;

public class InteractInteractionBehavior : IInteractableBehavior
{
    public InteractionType InteractionType => global::InteractionType.INTERACT;
    public Action<InteractionData> OnInteract;
    public void PerformInteraction(InteractionData data)
    {
        OnInteract?.Invoke(data);
    }
}
