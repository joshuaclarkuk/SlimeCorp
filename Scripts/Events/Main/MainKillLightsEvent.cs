using Godot;
using System;

public partial class MainKillLightsEvent : WorldEvent
{
    [ExportCategory("Required Nodes")]
    [Export] private Timer turnLightsBackOnTimerNode = null;

    private Node3D lightingHeader = null;

    public override void _Ready()
    {
        base._Ready();

        SubscribeToEvents();
    }

    public override void _ExitTree()
    {
        UnsubscribeFromEvents();
    }

    public override void SubscribeToEvents()
    {
        globalEvents.OnMainKillLightsEvent += HandleMainKillLightsEvent;
        turnLightsBackOnTimerNode.Timeout += HandleTurnLightsBackOnTimerNode;
    }

    public override void UnsubscribeFromEvents()
    {
        globalEvents.OnMainKillLightsEvent -= HandleMainKillLightsEvent;
    }

    private void HandleMainKillLightsEvent(Node3D lightingHeaderNode)
    {
        this.lightingHeader = lightingHeaderNode;

        foreach (Light3D light in lightingHeader.GetChildren())
        {
            light.Visible = false;
        }

        turnLightsBackOnTimerNode.Start();
    }

    private void HandleTurnLightsBackOnTimerNode()
    {
        foreach (Light3D light in lightingHeader.GetChildren())
        {
            light.Visible = true;
        }
    }
}