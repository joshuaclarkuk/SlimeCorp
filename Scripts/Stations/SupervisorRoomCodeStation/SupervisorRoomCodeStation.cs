using Godot;
using System;

public partial class SupervisorRoomCodeStation : Station
{
    [ExportCategory("Required Nodes")]
    [Export] private Card punchCardNode = null;

    public override void _Ready()
    {
        base._Ready();

        punchCardNode.Visible = false;
    }

    public override void EnterStation()
    {
        base.EnterStation();

        punchCardNode.OnCardTargetReached += HandleCardTargetReached;

        if (globalValues.HasSupervisorCard)
        {
            GD.Print("Display swipe card");
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

        if (isMouseClicked && globalValues.HasSupervisorCard)
        {
            punchCardNode.MovePhysicalCardWithMouseMotion(mouseDragMotion.Y, mouseDragSensitivity);
        }
    }

    private void HandleCardTargetReached()
    {
        GD.Print("Card target reached");
    }

    protected override void HandleButtonDisengaged(int buttonIndex)
    {
        
    }

    protected override void HandleButtonEngaged(int buttonIndex)
    {
        
    }
}
