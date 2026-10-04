public interface IInteractableBehavior
{
    InteractionType InteractionType { get; }
    void PerformInteraction(InteractionData data);
}
