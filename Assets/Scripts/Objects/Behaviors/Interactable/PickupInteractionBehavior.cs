public class PickupInteractionBehavior : IInteractableBehavior
{
    public InteractionType InteractionType => global::InteractionType.PICKUP;

    // The Interactor owns pickup and drop state, so this behavior only identifies the type.
    public void PerformInteraction(InteractionData data) { }
}
