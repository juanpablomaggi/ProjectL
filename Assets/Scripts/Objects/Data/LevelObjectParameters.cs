using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class LevelObjectParameters
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public MovableType movableType;
    public MovementType movementType;
    public ColliderLevel colliderLevel;

    [Header("Size")]
    public int width = 1;
    public int length = 1;
    public bool isEdgeObject;

    [Header("Interaction")]
    public InteractionType interactionType;

    [Header("Light Emmision")]
    public LightShape lightShape;
    public int lightRange;

    [Header("Light Block")]
    public LightBlockType lightBlockType;

    [Header("Activable")]
    public ActivationType activationType;
    public bool isPowered;
    public List<string> linkedElements = new List<string>();

    [Header("States")]
    public bool hasStates;
    public bool initialState;

    public LevelObjectParameters Clone()
    {
        LevelObjectParameters newObject = new LevelObjectParameters();
        newObject.moveSpeed = moveSpeed;
        newObject.movableType = movableType;
        newObject.movementType = movementType;
        newObject.colliderLevel = colliderLevel;

        newObject.width = width;
        newObject.length = length;
        newObject.isEdgeObject = isEdgeObject;

        newObject.interactionType = interactionType;
        
        newObject.lightShape = lightShape;
        newObject.lightRange = lightRange;

        newObject.lightBlockType = lightBlockType;

        newObject.activationType = activationType;
        newObject.isPowered = isPowered;

        var lnkElements = new List <string>();
        lnkElements.AddRange(linkedElements);
        newObject.linkedElements = lnkElements;

        newObject.hasStates = hasStates;
        newObject.initialState = initialState;

        return newObject;
    }
}
