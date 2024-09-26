using Godot;

public abstract partial class ControlState : Node
{
    [Export] public E_StationType stationType;

    public abstract override void _UnhandledInput(InputEvent @event);
}