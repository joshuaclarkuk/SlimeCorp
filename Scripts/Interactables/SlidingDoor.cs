using Godot;
using System;

public partial class SlidingDoor : StaticBody3D
{
    [ExportCategory("External Nodes")]
    [Export] private SupervisorRoomCodeStation supervisorRoomCodeStationNode = null;

    [ExportCategory("Required Nodes")]
    [Export] private AnimationPlayer animationPlayerNode = null;

    public override void _Ready()
    {
        supervisorRoomCodeStationNode.OnToggleDoorOpen += HandleToggleDoor;
    }

    public override void _ExitTree()
    {
        supervisorRoomCodeStationNode.OnToggleDoorOpen -= HandleToggleDoor;
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
