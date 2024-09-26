using Godot;

public partial class ClockOutControlState : ControlState
{
    public override void _UnhandledInput(InputEvent @event)
    {
        GD.Print($"{Name} unhandled input active");
    }
}