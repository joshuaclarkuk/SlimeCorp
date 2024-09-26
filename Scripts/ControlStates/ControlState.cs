using Godot;

public abstract partial class ControlState : Node
{
    [Export] public E_StationType stationType { get; private set; }

    public abstract override void _UnhandledInput(InputEvent @event);
}