using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Vector3 heldLocalOffset = new Vector3(0f, 2f, 0f);
    [SerializeField] private LevelObject playerLevelObject;

    private LevelObject heldObject;

    public bool HasPickup => heldObject != null;

    private void OnEnable()
    {
        if (playerLevelObject == null)
            playerLevelObject = GetComponent<LevelObject>();

        if (playerLevelObject == null)
            return;

        playerLevelObject.OnRemovedFromTile += OnPlayerRemovedFromTile;
        playerLevelObject.OnPlacedOnTile += OnPlayerPlacedOnTile;
        playerLevelObject.OnOrientationChanged += OnPlayerOrientationChanged;
    }

    private void OnDisable()
    {
        if (playerLevelObject == null)
            return;

        RemoveHeldLight(playerLevelObject.GridPosition);
        ClearHeldLightBlocker();
        playerLevelObject.OnRemovedFromTile -= OnPlayerRemovedFromTile;
        playerLevelObject.OnPlacedOnTile -= OnPlayerPlacedOnTile;
        playerLevelObject.OnOrientationChanged -= OnPlayerOrientationChanged;
    }

    public void Act()
    {
        if (playerLevelObject == null || playerLevelObject.Map == null || playerLevelObject.CurrentTile == null)
            return;

        if (HasPickup)
        {
            TryDrop();
            return;
        }

        Tile targetTile = GetFacingTile();
        if (targetTile == null)
            return;

        if (TryInteract(targetTile))
            return;

        TryPickUp(targetTile);
    }

    private bool TryInteract(Tile targetTile)
    {
        LevelObject target = GetObjectWithInteractionType(targetTile, InteractionType.INTERACT);
        if (target == null)
            return false;

        InteractionData interactionData = InteractionData.Create(
            playerTransform: playerTransform,
            playerTile: playerLevelObject.GridPosition,
            targetTile: targetTile.GridPosition
        );

        target.GetBehavior<InteractableComponent>().Interact(interactionData);
        return true;
    }

    private bool TryPickUp(Tile targetTile)
    {
        LevelObject pickup = GetObjectWithInteractionType(targetTile, InteractionType.PICKUP);
        if (pickup == null || !targetTile.RemoveObject(pickup))
            return false;

        heldObject = pickup;

        SetHeldObjectCollidersEnabled(false);
        heldObject.transform.position = playerTransform.position + heldLocalOffset;
        heldObject.transform.localRotation = Quaternion.identity;
        playerLevelObject.GetBehavior<MovableComponent>().AttachElement(heldObject);
        CopyHeldLightBlocker();
        ApplyHeldLight(playerLevelObject.GridPosition);
        return true;
    }

    private bool TryDrop()
    {
        Tile targetTile = GetFacingTile();
        if (targetTile == null || !targetTile.IsEmpty(heldObject.Layer))
            return false;

        RemoveHeldLight(playerLevelObject.GridPosition);
        ClearHeldLightBlocker();

        playerLevelObject.GetBehavior<MovableComponent>().RemoveElement(heldObject);
        heldObject.transform.position = targetTile.WorldPosition;
        if (!targetTile.TryPlaceObject(heldObject.Layer, heldObject))
        {
            heldObject.transform.position = playerTransform.position + heldLocalOffset;
            playerLevelObject.GetBehavior<MovableComponent>().AttachElement(heldObject);
            CopyHeldLightBlocker();
            ApplyHeldLight(playerLevelObject.GridPosition);
            return false;
        }

        SetHeldObjectCollidersEnabled(true);
        heldObject = null;
        return true;
    }

    private Tile GetFacingTile()
    {
        return playerLevelObject.Map.GetTile(playerLevelObject.GridPosition + playerLevelObject.Orientation.ToVector2Int());
    }

    private LevelObject GetObjectWithInteractionType(Tile tile, InteractionType interactionType)
    {
        foreach (TileLayer layer in System.Enum.GetValues(typeof(TileLayer)))
        {
            LevelObject levelObject = tile.GetObject(layer);
            InteractableComponent interactable = levelObject?.GetBehavior<InteractableComponent>();
            if (interactable != null && interactable.InteractionType == interactionType)
                return levelObject;
        }

        return null;
    }

    private void SetHeldObjectCollidersEnabled(bool enabled)
    {
        foreach (Collider collider in heldObject.GetComponentsInChildren<Collider>())
            collider.enabled = enabled;
    }

    private void OnPlayerRemovedFromTile(Tile tile)
    {
        RemoveHeldLight(tile.GridPosition);
    }

    private void OnPlayerPlacedOnTile(Tile tile)
    {
        ApplyHeldLight(tile.GridPosition);
    }

    private void OnPlayerOrientationChanged(Direction direction)
    {
        if (!HasPickup || playerLevelObject.CurrentTile == null)
            return;

        RemoveHeldLight(playerLevelObject.GridPosition);
        ApplyHeldLight(playerLevelObject.GridPosition);
    }

    private void ApplyHeldLight(Vector2Int position)
    {
        if (!HasPickup || playerLevelObject.Map == null)
            return;

        heldObject.GetBehavior<LightEmmiterComponent>()?.ApplyIllumination(
            position,
            playerLevelObject.Orientation,
            playerLevelObject.Map,
            playerLevelObject);
    }

    private void RemoveHeldLight(Vector2Int position)
    {
        if (!HasPickup || playerLevelObject.Map == null)
            return;

        heldObject.GetBehavior<LightEmmiterComponent>()?.RemoveIllumination(
            position,
            playerLevelObject.Orientation,
            playerLevelObject.Map,
            playerLevelObject);
    }

    private void CopyHeldLightBlocker()
    {
        playerLevelObject.GetBehavior<LightBlockerComponent>()?.CopyFrom(
            heldObject.GetBehavior<LightBlockerComponent>());
    }

    private void ClearHeldLightBlocker()
    {
        playerLevelObject.GetBehavior<LightBlockerComponent>()?.ClearCopiedBehavior();
    }
}
