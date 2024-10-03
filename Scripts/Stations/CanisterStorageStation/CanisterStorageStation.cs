using Godot;
using System;
using System.Reflection.Metadata;

public partial class CanisterStorageStation : Station
{
    [ExportCategory("Required Nodes")]
    [Export] private Lever leverNode = null;

    private bool playerHasTakenBarrel = false;
    private bool barrelIsAttachedToCollectionStation = true;

    public override void _Ready()
    {
        base._Ready();

        globalSignals.OnSlimeCanisterRemovedFromStation += HandleOnSlimeCanisterRemovedFromStation;
        globalSignals.OnSlimeCanisterAddedToStation += HandleSlimeCanisterAddedToStation;
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        globalSignals.OnSlimeCanisterRemovedFromStation -= HandleOnSlimeCanisterRemovedFromStation;
        globalSignals.OnSlimeCanisterAddedToStation -= HandleSlimeCanisterAddedToStation;
    }

    public override void EnterStation()
    {
        base.EnterStation();

        // Link signals
        leverNode.OnLeverTargetReached += HandleLeverTargetReached;

        CheckIfBarrelCollectionIsValid();

        GD.Print($"Calling EnterStation method on {Name}");
    }

    public override void ExitStation()
    {
        base.ExitStation();

        // Unlink signals
        leverNode.OnLeverTargetReached -= HandleLeverTargetReached;

        // Reset machine
        ResetMachine();

        GD.Print($"Calling ExitStation method on {Name}");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

        if (isMouseClicked)
        {
            if (canInteractWithStation)
            {
                leverNode.MovePhysicalHandleWithMouseMotion(mouseDragSensitivity, mouseDragMotion);
            }
        }
        else
        {
            leverNode.ReturnToOriginalPosition();
        }
    }

    public void ResetMachine()
    {
        leverNode.ReturnToOriginalPosition();
    }

    private void CheckIfBarrelCollectionIsValid()
    {
        if (barrelIsAttachedToCollectionStation)
        {
            GD.PrintErr("Can't collect barrel, barrel attached to station already");
            globalSignals.RaisePlayerExitStation(StationType);
        }
    }

    private void HandleLeverTargetReached()
    {
        playerHasTakenBarrel = true;
        globalSignals.RaiseSlimeCanisterTakenFromStorage();
        globalSignals.RaisePlayerExitStation(StationType);
    }

    private void HandleOnSlimeCanisterRemovedFromStation(float slimeAmount)
    {
        barrelIsAttachedToCollectionStation = false;
    }

    private void HandleSlimeCanisterAddedToStation()
    {
        barrelIsAttachedToCollectionStation = true;
    }

    protected override void HandleButtonDisengaged(int buttonIndex)
    {

    }

    protected override void HandleButtonEngaged(int buttonIndex)
    {

    }
}
