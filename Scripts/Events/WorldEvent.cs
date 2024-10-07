using Godot;
using System;

public abstract partial class WorldEvent : Node
{
    protected GlobalEvents globalEvents;

    public override void _Ready()
    {
        globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");
    }

    public abstract void SubscribeToEvents();
    public abstract void UnsubscribeFromEvents();
}