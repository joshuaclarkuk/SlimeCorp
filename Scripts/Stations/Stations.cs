using Godot;
using Godot.Collections;
using System;

public partial class Stations : Node3D
{
    [ExportCategory("Required Nodes")]
    [Export] private Camera3D rovingCameraNode = null;
    [Export] private Player playerNode = null;

    [ExportCategory("Behaviour")]
    [Export] private float cameraTweenDuration = 1.0f;

    private GlobalSignals globalSignals = null;
    private Dictionary<E_StationType, Marker3D> cameraPositionDictionary = new Dictionary<E_StationType, Marker3D>();

    // Member variable for player camera position
    private Node3D playerCameraPivotNode;

    public override void _Ready()
    {
        globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");
        globalSignals.OnPlayerInteractWithStation += HandlePlayerInteractWithStation;
        globalSignals.OnPlayerExitStation += HandlePlayerExitStation;

        // Initialise dictionary
        foreach(Station station in GetChildren())
        {
            cameraPositionDictionary.Add(station.StationType, station.GetNode<Marker3D>("CameraLocation"));
        }

        // Get reference to player camera position
        playerCameraPivotNode = playerNode.GetNode<Node3D>("PlayerCameraPivot");
    }

    public override void _ExitTree()
    {
        globalSignals.OnPlayerInteractWithStation -= HandlePlayerInteractWithStation;
        globalSignals.OnPlayerExitStation -= HandlePlayerExitStation;
    }

    private void HandlePlayerInteractWithStation(E_StationType type)
    {
        // Try to find the target transform in the dictionary
        if (cameraPositionDictionary.TryGetValue(type, out Marker3D targetMarker))
        {
            // Set roving camera to player's position
            rovingCameraNode.GlobalTransform = playerCameraPivotNode.GlobalTransform;

            // Switch active camera to roving camera
            rovingCameraNode.MakeCurrent();

            // Create and execute camera tweens
            // Position
            Tween moveCameraToStationPositionTween = GetTree().CreateTween();
            moveCameraToStationPositionTween.SetTrans(Tween.TransitionType.Sine);
            moveCameraToStationPositionTween.TweenProperty(rovingCameraNode, "position", targetMarker.GlobalPosition, cameraTweenDuration);
            moveCameraToStationPositionTween.Play();
            // Rotation
            Tween moveCameraToStationRotationTween = GetTree().CreateTween();
            moveCameraToStationRotationTween.SetTrans(Tween.TransitionType.Sine);
            moveCameraToStationRotationTween.TweenProperty(rovingCameraNode, "rotation", targetMarker.GlobalRotation, cameraTweenDuration);
            moveCameraToStationRotationTween.Play();
        }
        else
        {
            GD.PrintErr($"Station type {type} not found in camera position dictionary.");
        }
    }

    private void HandlePlayerExitStation(E_StationType type)
    {
        // Create and execute camera tweens
        // Position
        Tween returnCameraPositionTween = GetTree().CreateTween();
        returnCameraPositionTween.SetTrans(Tween.TransitionType.Sine);
        returnCameraPositionTween.TweenProperty(rovingCameraNode, "position", playerCameraPivotNode.GlobalPosition, cameraTweenDuration);
        returnCameraPositionTween.Play();
        returnCameraPositionTween.Finished += HandleReturnCameraTweenFinished;
        // Rotation
        Tween returnCameraRotationTween = GetTree().CreateTween();
        returnCameraRotationTween.SetTrans(Tween.TransitionType.Sine);
        returnCameraRotationTween.TweenProperty(rovingCameraNode, "rotation", playerCameraPivotNode.GlobalRotation, cameraTweenDuration);
        returnCameraRotationTween.Play();
    }

    private void HandleReturnCameraTweenFinished()
    {
        globalSignals.RaisePlayerCanMoveAgain();
    }
}
