using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileRestrictedMovementBehavior : IMovableBehavior
{
    private readonly float speed;
    private bool isMoving;

    public TileRestrictedMovementBehavior(float speed = 5f)
    {
        this.speed = speed;
    }

    public bool IsMovable => true;
    public bool IsMoving => isMoving;

    public bool TryMove(LevelObject obj, Vector2 direction)
    {
        return obj.Map != null && obj.Map.TryMoveToAdjacentTile(obj, direction.ToDirection());
    }

    public bool CanMove(MovementData data)
    {
        return data.OriginTile != null &&
               data.TargetTile != null &&
               data.ChangesTile;
    }

    public void Move(MovementData data)
    {
        data.Object.StartCoroutine(
            MoveRoutine(
                data.Object.transform,
                data.TargetWorldPosition,
                data.LinkedObjects));
    }

    private IEnumerator MoveRoutine(
        Transform obj,
        Vector3 targetPosition,
        IEnumerable<LevelObject> linkedObjects)
    {
        isMoving = true;
        var linkedTransforms = new List<Transform>();
        var offsets = new List<Vector3>();

        if (linkedObjects != null)
        {
            foreach (LevelObject linkedObject in linkedObjects)
            {
                if (linkedObject == null)
                    continue;

                linkedTransforms.Add(linkedObject.transform);
                offsets.Add(linkedObject.transform.position - obj.position);
            }
        }

        while (Vector3.Distance(
            obj.position,
            targetPosition) > 0.01f)
        {
            obj.position = Vector3.MoveTowards(
                obj.position,
                targetPosition,
                speed * Time.deltaTime);

            MoveLinkedObjects(obj.position, linkedTransforms, offsets);

            yield return null;
        }

        obj.position = targetPosition;
        MoveLinkedObjects(obj.position, linkedTransforms, offsets);
        isMoving = false;
    }

    private static void MoveLinkedObjects(
        Vector3 origin,
        IReadOnlyList<Transform> linkedTransforms,
        IReadOnlyList<Vector3> offsets)
    {
        for (int i = 0; i < linkedTransforms.Count; i++)
        {
            if (linkedTransforms[i] != null)
                linkedTransforms[i].position = origin + offsets[i];
        }
    }
}
