using Godot;
using System;

public partial class SupervisorCardPickupStation : Station
{
    [ExportCategory("Required Nodes")]
    [Export] private Control pickupUINode = null;

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
            GD.Print("Has picked up keycard");
        }

        pickupUINode.Visible = false;

    }

    protected override void HandleButtonDisengaged(int buttonIndex)
    {
        
    }

    protected override void HandleButtonEngaged(int buttonIndex)
    {
        
    }
}
