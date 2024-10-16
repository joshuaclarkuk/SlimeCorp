using Godot;
using System;

public partial class PoisonCreatureStation : Station
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

        if (CanInjectCreature())
        {
            punchCardNode.Visible = true;
        }
        else
        {
            punchCardNode.Visible = false;
        }
    }

    public override void ExitStation()
    {
        base.ExitStation();

        punchCardNode.OnCardTargetReached -= HandleCardTargetReached;

        punchCardNode.Visible = false;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

        if (isMouseClicked && CanInjectCreature())
        {
            punchCardNode.MovePhysicalCardWithMouseMotion(mouseDragMotion.Y, mouseDragSensitivity);
        }
        else
        {
            punchCardNode.ReturnToOriginalPosition();
        }
    }

    private bool CanInjectCreature()
    {
        return globalValues.HasPoisonInjector && !globalValues.HasPlayerInjectedCreature;
    }

    private void HandleCardTargetReached()
    {
        globalValues.SetHasPlayerInjectedCreature(true);
        globalSignals.RaisePlayerHasInjectedCreatureWithPoison();
        globalSignals.RaisePlayerExitStation(StationType);
        GD.Print("Injected creature with poison. Clock out to WIN!");
    }

    protected override void HandleButtonDisengaged(int buttonIndex)
    {
        throw new NotImplementedException();
    }

    protected override void HandleButtonEngaged(int buttonIndex)
    {
        throw new NotImplementedException();
    }
}
