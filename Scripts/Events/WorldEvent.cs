using Godot;

public abstract partial class WorldEvent : Node
{
    public abstract void StartWorldEvent();
    public abstract void EndWorldEvent();
}