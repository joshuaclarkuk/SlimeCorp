using Godot;
using System;

public partial class SupervisorRoomCodeStation : Station
{
    [ExportCategory("Required Nodes")]
    [Export] private Card punchCardNode = null;
    [Export] private Timer doorOpenLockoutTimerNode = null;

    private bool isDoorOpen = false;
    private bool hasAlreadyBeenAccessed = false;

    public event Action<bool> OnToggleDoorOpen;

    public override void _Ready()
    {
        base._Ready();

        globalSignals.OnPlayerClockedOut += HandePlayerClockedOut;

        punchCardNode.Visible = false;
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        globalSignals.OnPlayerClockedOut -= HandePlayerClockedOut;
    }

    public override void EnterStation()
    {
        base.EnterStation();

        punchCardNode.OnCardTargetReached += HandleCardTargetReached;

        if (globalValues.HasSupervisorCard)
        {
            punchCardNode.Visible = true;
        }
        else
        {
            GD.PrintErr("No supervisor key card");
            punchCardNode.Visible = false;
        }
    }

    public override void ExitStation()
    {
        base.ExitStation();

        punchCardNode.OnCardTargetReached -= HandleCardTargetReached;

        punchCardNode.Visible = false;
        punchCardNode.ReturnToOriginalPosition();

    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

        if (!globalValues.HasSupervisorCard) { return; }

        if (isMouseClicked)
        {
            punchCardNode.MovePhysicalCardWithMouseMotion(mouseDragMotion.Y, mouseDragSensitivity);
        }
        else
        {
            punchCardNode.ReturnToOriginalPosition();
        }
    }

    private void HandleCardTargetReached()
    {
        // Used to activate poison injector pickup
        if (!hasAlreadyBeenAccessed)
        {
            globalSignals.RaisePlayerAccessedSupervisorOffice();
            hasAlreadyBeenAccessed = true;
        }

        if (doorOpenLockoutTimerNode.TimeLeft > 0.0f) { return; }

        GD.Print("Card target reached");
        if (!isDoorOpen)
        {
            OnToggleDoorOpen(true);
            isDoorOpen = true;
            doorOpenLockoutTimerNode.Start();
        }
        else
        {
            OnToggleDoorOpen(false);
            isDoorOpen = false;
            doorOpenLockoutTimerNode.Start();
        }
        globalSignals.RaisePlayerExitStation(StationType);
    }

    private void HandePlayerClockedOut()
    {
        GD.Print("CLOSING DOOR");
        OnToggleDoorOpen?.Invoke(false);
        isDoorOpen = false;
    }

    protected override void HandleButtonDisengaged(int buttonIndex)
    {
        
    }

    protected override void HandleButtonEngaged(int buttonIndex)
    {
        
    }
}
