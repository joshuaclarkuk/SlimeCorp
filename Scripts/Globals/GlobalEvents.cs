using Godot;
using System;

public partial class GlobalEvents : Node
{
    // LOBBY

    // MAIN
    public event Action<Node3D> OnMainKillLightsEvent;

    // MAIN
    public void RaiseMainKillLightsEvent(Node3D lightingHeaderNode) { OnMainKillLightsEvent?.Invoke(lightingHeaderNode); }

    //LOBBY
}