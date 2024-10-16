using Godot;
using System;

public partial class RotatingDoor : StaticBody3D
{
    [ExportCategory("External Nodes")]
    [Export] private AbandonedRoomCodeStation abandonedRoomCodeStationNode = null;

    [ExportCategory("Required Nodes")]
    [Export] private AnimationPlayer animationPlayerNode = null;

    public override void _Ready()
    {
        abandonedRoomCodeStationNode.OnToggleDoorOpen += HandleToggleDoor;
    }

    public override void _ExitTree()
    {
        abandonedRoomCodeStationNode.OnToggleDoorOpen -= HandleToggleDoor;
    }

    private void HandleToggleDoor(bool shouldOpen)
    {
        if (shouldOpen)
        {
            animationPlayerNode.Play("OpenDoor");
        }
        else
        {
            animationPlayerNode.PlayBackwards("OpenDoor");
        }
    }
}
