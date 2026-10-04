using System;
using UnityEngine;

public class InteractableComponent : IObjectBehavior
{
    private IInteractableBehavior interactableBehavior;
    public InteractionType InteractionType => interactableBehavior?.InteractionType ?? InteractionType.NONE;

    public void Configure(LevelObjectParameters data)
    {
        switch(data.interactionType)
        {
            case InteractionType.PICKUP:
                interactableBehavior = new PickupInteractionBehavior();
                break;
            case InteractionType.INTERACT:
                interactableBehavior = new InteractInteractionBehavior();
                break;
            case InteractionType.NONE:
            default:
                interactableBehavior = new NoInteractionBehavior();
                break;
        }
    }

    public void Interact(InteractionData interactionData)
    {
        interactableBehavior.PerformInteraction(interactionData);
    }

    public void SetupAction(Action<InteractionData> action)
    {
        if (interactableBehavior is InteractInteractionBehavior actionBehavior)
        {
            actionBehavior.OnInteract += action;
        }
    }
}

public enum InteractionType { NONE, PICKUP, INTERACT }
