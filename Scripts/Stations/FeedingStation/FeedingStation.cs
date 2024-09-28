using Godot;

public partial class FeedingStation : Station
{
    public override void _UnhandledInput(InputEvent @event)
    {
        GD.Print($"{Name}: Unhandled Input active");
    }
}