using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : LevelObject
{
    [SerializeField] private Interactor interactor;
    [SerializeField] private CollisionHandler collisionHandler;
    [SerializeField] private TriggerHandler triggerHandler;
    private InputSystemActions input;
    private PlayerModel model;
    private PlayerView view;
    private Vector2 movementInput;

    private void Awake()
    {
        view = GetComponent<PlayerView>();
        input = new InputSystemActions();
    }

    public override void Initialize(LevelObjectData data, GridMap gridMap)
    {
        base.Initialize(data, gridMap);

        model = new PlayerModel(this);
        input.Player.Move.started += OnMove;
        input.Player.Move.performed += OnMove;
        input.Player.Move.canceled += OnMove;
        input.Player.Interact.started += OnInteract;

        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
        input.Player.Move.started -= OnMove;
        input.Player.Move.performed -= OnMove;
        input.Player.Move.canceled -= OnMove;
        input.Player.Interact.started -= OnInteract;

    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        movementInput = ctx.ReadValue<Vector2>();
    }

    private void Update()
    {
        if (model != null && movementInput.sqrMagnitude > 0.0001f)
            model.TryMove(movementInput);
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (interactor != null)
            interactor.Act();
    }
}
