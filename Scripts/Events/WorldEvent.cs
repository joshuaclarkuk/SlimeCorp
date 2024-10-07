using Godot;
using System;

public abstract partial class WorldEvent : Node
{
    protected GlobalEvents globalEvents;
    protected GlobalValues globalValues;
    protected GlobalSignals globalSignals;

    public override void _Ready()
    {
        globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");
        globalValues = GetNode<GlobalValues>("/root/GlobalValues");
        globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");
    }

    public abstract void SubscribeToEvents();
    public abstract void UnsubscribeFromEvents();
}