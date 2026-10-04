public class NoInteractionBehavior : IInteractableBehavior
{
    public InteractionType InteractionType => global::InteractionType.NONE;
    public void PerformInteraction(InteractionData data) {}
}
