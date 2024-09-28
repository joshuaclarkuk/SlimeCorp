using Godot;
using System;

public partial class SlimeCollectionStation : Station
{
    public override void _UnhandledInput(InputEvent @event)
    {
        GD.Print($"{Name}: Unhandled Input active");
    }
}
