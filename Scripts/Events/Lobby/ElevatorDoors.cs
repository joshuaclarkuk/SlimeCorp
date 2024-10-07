using Godot;
using System;

public partial class ElevatorDoors : Node3D
{
    [ExportCategory("Required Nodes")]
    [Export] private Area3D playerStandInElevatorNode = null;
    [Export] private AnimationPlayer elevatorDoorsAnimationPlayerNode = null;
    [Export] private PackedScene sceneToTransitionTo = null;

    private bool areOpening = false;

    public override void _Ready()
    {
        playerStandInElevatorNode.BodyEntered += HandlePlayerStandInElevatorBodeEntered;
    }

    public override void _ExitTree()
    {
        playerStandInElevatorNode.BodyEntered -= HandlePlayerStandInElevatorBodeEntered;
    }

    public void ToggleElevatorDoorsOpen(bool isOpen)
    {
        if (areOpening) { return; }

        if (isOpen)
        {
            elevatorDoorsAnimationPlayerNode.Play("OpenElevatorDoors");
            areOpening = true;
            elevatorDoorsAnimationPlayerNode.AnimationFinished += HandleElevatorDoorsAnimationPlayerAnimationFinished;
        }
        else
        {
            elevatorDoorsAnimationPlayerNode.PlayBackwards("OpenElevatorDoors");
            areOpening = true;
            elevatorDoorsAnimationPlayerNode.AnimationFinished += HandleElevatorDoorsAnimationPlayerAnimationFinished;
        }
    }

    private void HandlePlayerStandInElevatorBodeEntered(Node3D body)
    {
        ToggleElevatorDoorsOpen(false);
        this.CallDeferred(nameof(ActivateElevatorRide));
    }

    private void HandleElevatorDoorsAnimationPlayerAnimationFinished(StringName animName)
    {
        if (animName == "OpenElevatorDoors")
        {
            GD.Print("Elevator animation finished");
            areOpening = false;
            elevatorDoorsAnimationPlayerNode.AnimationFinished -= HandleElevatorDoorsAnimationPlayerAnimationFinished;
        }
    }

    private void ActivateElevatorRide()
    {
        GD.Print("Activating elevator");
        // Move roving camera to position in lift
        // Add timer
        // Add camera shake
        // Add sounds
        // Make lights flicker
        // Transition scene
        GetTree().ChangeSceneToPacked(sceneToTransitionTo);
    }
}
