using Godot;
using System;

public partial class SupervisorCardPickupStation : Station
{
    [ExportCategory("Required Nodes")]
    [Export] private Control pickupUINode = null;
    [Export] private MeshInstance3D meshNode = null;
    [Export] private CollisionShape3D colliderNode = null;
    [Export] private ComputerItemResource poisonInjectorEmailResource = null;

    private bool hasPickedUpKeycard = false;

    public override void _Ready()
    {
        base._Ready();

        pickupUINode.Visible = false;
    }

    public override void EnterStation()
    {
        base.EnterStation();

        pickupUINode.Visible = true;
    }

    public override void ExitStation()
    {
        base.ExitStation();

        if (!hasPickedUpKeycard)
        {
            hasPickedUpKeycard = true;
            globalValues.SetHasSupervisorCard(true);
            globalSignals.RaisePlayerHasSupervisorCard();
            GD.Print("Has picked up keycard");
        }

        pickupUINode.Visible = false;
        meshNode.Visible = false;
        colliderNode.Disabled = true;
        globalSignals.RaiseEmailReceived(poisonInjectorEmailResource);
    }

    protected override void HandleButtonDisengaged(int buttonIndex)
    {
        
    }

    protected override void HandleButtonEngaged(int buttonIndex)
    {
        
    }
}
