using Godot;
using System;

public partial class ClockOutStation : Station
{
    public override void _UnhandledInput(InputEvent @event)
    {
        GD.Print($"{Name}: Unhandled Input active");
    }
}
