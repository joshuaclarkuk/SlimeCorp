using Godot;
using System;

public partial class CleaningStation : Station
{
    public override void EnterStation()
    {
        GD.Print($"Calling EnterStation method on {Name}");
    }

    public override void ExitStation()
    {
        GD.Print($"Calling ExitStation method on {Name}");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        GD.Print($"{Name}: Unhandled Input active");
    }
}
