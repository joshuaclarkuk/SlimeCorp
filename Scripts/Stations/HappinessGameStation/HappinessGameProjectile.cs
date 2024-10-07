using Godot;
using System;

public partial class HappinessGameProjectile : Area2D
{
    [ExportCategory("Required Nodes")]
    [Export] private Timer selfDestructTimerNode = null;

    [ExportCategory("Behaviour")]
    [Export] private float movementSpeed = 1.0f;

    private GlobalSignals globalSignals = null;

    public override void _EnterTree()
    {
        BodyEntered += HandleBodyEntered;

        selfDestructTimerNode.Timeout += HandleSelfDestructTimerTimeout;
    }

    public override void _ExitTree()
    {
        BodyEntered -= HandleBodyEntered;
        selfDestructTimerNode.Timeout -= HandleSelfDestructTimerTimeout;
    }

    public override void _PhysicsProcess(double delta)
    {
        TravelUpwards(delta);
    }

    public void SetGlobalSignals(GlobalSignals globalSignals)
    {
        this.globalSignals = globalSignals;
    }

    private void TravelUpwards(double delta)
    {
        float newPosY = Position.Y;
        newPosY -= movementSpeed * (float)delta;
        Position = new Vector2(Position.X, newPosY);
    }

    private void HandleBodyEntered(Node2D body)
    {
        if (body is HappinessGameEnemy)
        {
            HappinessGameEnemy enemy = (HappinessGameEnemy)body;
            enemy.ToggleEnemy(false);
            globalSignals.RaiseCreaturePlayedWith();
            QueueFree();
        }
    }

    private void HandleSelfDestructTimerTimeout()
    {
        QueueFree();
    }
}
