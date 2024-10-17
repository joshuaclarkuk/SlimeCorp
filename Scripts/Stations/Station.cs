using Godot;
using System;

public abstract partial class Station : Node3D
{
    [ExportCategory("Required Nodes")]
    [Export] protected KeypadButtons buttonsNode = null;
    [Export] protected StationControlInstructions stationControlInstructionsNode = null;
    [Export] private Label3D debugLabel = null;

    [ExportCategory("Mouse Settings")]
    [Export] protected float mouseDragSensitivity = 0.05f;

    [ExportCategory("Station Type")]
    [Export] public E_StationType StationType { get; private set; }

    [ExportCategory("Tutorial Prompts")]
    [Export] protected bool hasNumPad = false;
    [Export] protected bool isHappinessGame = false;
    [Export] protected bool hasMouseDrag = false;

    protected GlobalSignals globalSignals = null;
    protected GlobalValues globalValues = null;
    private Area3D interactColliderNode = null;

    protected Vector2 mouseDragMotion = Vector2.Zero;
    protected bool isMouseClicked = false;
    protected bool canInteractWithStation = true; // Use to lock a station out from interaction (after serving food, for example)

    private bool hasBeenVisitedBefore = false;

    public override void _Ready()
    {
        globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");
        globalValues = GetNode<GlobalValues>("/root/GlobalValues");
        interactColliderNode = GetNode<Area3D>("InteractCollider");

        // Set debug label
        debugLabel.Text = StationType.ToString();

        // Link button pressed signals with station behaviour
        buttonsNode.OnButtonEngaged += HandleButtonEngaged;
        buttonsNode.OnButtonDisengaged += HandleButtonDisengaged;

        interactColliderNode.BodyEntered += HandleInteractColliderNodeAreaEntered;
        interactColliderNode.BodyExited += HandleInteractColliderNodeAreaExited;

        DisableStationControlInstructions();
    }

    public override void _ExitTree()
    {
        interactColliderNode.BodyEntered -= HandleInteractColliderNodeAreaEntered;
        interactColliderNode.BodyExited -= HandleInteractColliderNodeAreaExited;
    }

    public virtual void EnterStation()
    {
        Input.MouseMode = Input.MouseModeEnum.Visible;

        stationControlInstructionsNode.SetMouseDragIconVisible(hasMouseDrag);
        stationControlInstructionsNode.SetHappinessGameIconsVisible(isHappinessGame);
        stationControlInstructionsNode.SetNumPadIconsVisible(hasNumPad);

        if (!hasBeenVisitedBefore)
        {
            stationControlInstructionsNode.ActivateTutorialNode(StationType);
            hasBeenVisitedBefore = true;
        }
    }

    public virtual void ExitStation()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        isMouseClicked = false;

        DisableStationControlInstructions();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (Input.IsActionJustPressed(GlobalConstants.INPUT_NUM_0)) { PushButton(0); }
        if (Input.IsActionJustPressed(GlobalConstants.INPUT_NUM_1)) { PushButton(1); }
        if (Input.IsActionJustPressed(GlobalConstants.INPUT_NUM_2)) { PushButton(2); }
        if (Input.IsActionJustPressed(GlobalConstants.INPUT_NUM_3)) { PushButton(3); }
        if (Input.IsActionJustPressed(GlobalConstants.INPUT_NUM_4)) { PushButton(4); }
        if (Input.IsActionJustPressed(GlobalConstants.INPUT_NUM_5)) { PushButton(5); }
        if (Input.IsActionJustPressed(GlobalConstants.INPUT_NUM_6)) { PushButton(6); }
        if (Input.IsActionJustPressed(GlobalConstants.INPUT_NUM_7)) { PushButton(7); }
        if (Input.IsActionJustPressed(GlobalConstants.INPUT_NUM_8)) { PushButton(8); }
        if (Input.IsActionJustPressed(GlobalConstants.INPUT_NUM_9)) { PushButton(9); }

        // Click and drag mouse
        if (Input.IsActionJustPressed(GlobalConstants.INPUT_MOUSE_1))
        {
            isMouseClicked = true;
        }
        else if (Input.IsActionJustReleased(GlobalConstants.INPUT_MOUSE_1))
        {
            isMouseClicked = false;
        }

        if (isMouseClicked)
        {
            // Calculate mouse motion
            if (@event is InputEventMouseMotion motion)
            {
                mouseDragMotion = -motion.Relative * mouseDragSensitivity;
            }
        }
    }

    protected abstract void HandleButtonEngaged(int buttonIndex);
    protected abstract void HandleButtonDisengaged(int buttonIndex);

    public void HandleInteractColliderNodeAreaEntered(Node3D body)
    {
        if (body is Player)
        {
            globalSignals.RaisePlayerEnterStationCollider(StationType);
        }        
    }

    private void DisableStationControlInstructions()
    {
        stationControlInstructionsNode.SetMouseDragIconVisible(false);
        stationControlInstructionsNode.SetHappinessGameIconsVisible(false);
        stationControlInstructionsNode.SetNumPadIconsVisible(false);

        stationControlInstructionsNode.DisableAllTutorialNodes();
    }

    private void PushButton(int buttonToPush)
    {
        buttonsNode.PushButton(buttonToPush);
    }

    private void HandleInteractColliderNodeAreaExited(Node3D body)
    {
       if (body is Player)
        {
            globalSignals.RaisePlayerExitStationCollider(StationType);
        }
    }
}