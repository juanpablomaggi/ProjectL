using UnityEngine;

public class PlayerModel
{
    private PlayerController player;
    private MovableComponent movable;

    public PlayerModel(PlayerController playerController)
    {
        player = playerController;
        movable = player.GetBehavior<MovableComponent>();
    }

    public void TryMove(Vector2 direction)
    {
        if (movable != null && movable.TryMove(player, direction))
        {
            //TODO Run animations or something
        }
        player.Rotate(direction.ToDirection());
    }

    public void TryInteract()
    {

        
    }
}
