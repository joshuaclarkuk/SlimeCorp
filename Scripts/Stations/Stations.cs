using Godot;
using Godot.Collections;
using System;

public partial class Stations : Node3D
{
    [ExportCategory("Required Nodes")]
    [Export] private Camera3D rovingCameraNode = null;

    [ExportCategory("Behaviour")]
    [Export] private float cameraTweenDuration = 1.0f;

    private GlobalSignals globalSignals = null;
    private Dictionary<E_StationType, Marker3D> cameraPositionDictionary = new Dictionary<E_StationType, Marker3D>();

    // Member variable for player camera position
    private Player player = null;
    private Node3D playerCameraPivotNode;

    public override void _Ready()
    {
        globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");
        player = GetNode<Player>("/root/Player");

        // Link signals with camera movement
        globalSignals.OnPlayerInteractWithStation += HandlePlayerInteractWithStation;
        globalSignals.OnPlayerExitStation += HandlePlayerExitStation;

        // Initialise camera position dictionary
        foreach(Station station in GetChildren())
        {
            cameraPositionDictionary.Add(station.StationType, station.GetNode<Marker3D>("CameraLocation"));
        }

        // Deactivate each station input callbacks on start
        DeactivateAllStationInputs();

        // Get reference to player camera position
        playerCameraPivotNode = player.GetNode<Node3D>("PlayerCameraPivot");
    }

    public override void _ExitTree()
    {
        globalSignals.OnPlayerInteractWithStation -= HandlePlayerInteractWithStation;
        globalSignals.OnPlayerExitStation -= HandlePlayerExitStation;
    }

    private void HandlePlayerInteractWithStation(E_StationType stationType)
    {
        MoveCameraToStation(stationType);
        ActivateRelevantStationInputs(stationType);
    }

    private void HandlePlayerExitStation(E_StationType stationType)
    {
        MoveCameraToPlayer();
        DeactivateAllStationInputs();
        CallStationExitMethod(stationType);
    }

    private void MoveCameraToStation(E_StationType stationType)
    {
        // Move camera to station position
        if (cameraPositionDictionary.TryGetValue(stationType, out Marker3D targetMarker))
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
            GD.PrintErr($"Station type {stationType} not found in camera position dictionary.");
        }
    }

    private void ActivateRelevantStationInputs(E_StationType stationType)
    {
        if (stationType == E_StationType.NONE)
        {
            foreach (Station station in GetChildren())
            {
                DeactivateStationInputs(station);
            }
        }
        else
        {
            foreach (Station station in GetChildren())
            {
                if (station.StationType == stationType)
                {
                    ActivateStationInputs(station);
                    CallStationEnterMethod(station);
                }
                else
                {
                    DeactivateStationInputs(station);
                }
            }
        }
    }

    private void MoveCameraToPlayer()
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

    private void ActivateStationInputs(Station station)
    {
        station.SetProcessInput(true);
        station.SetProcessUnhandledInput(true);
    }

    private void DeactivateStationInputs(Station station)
    {
        station.SetProcessInput(false);
        station.SetProcessUnhandledInput(false);
    }

    private void DeactivateAllStationInputs()
    {
        foreach (Station station in GetChildren())
        {
            DeactivateStationInputs(station);
        }
    }

    private void CallStationEnterMethod(Station station) { station.EnterStation(); }
    private void CallStationExitMethod(E_StationType stationType) 
    {
        foreach (Station station in GetChildren())
        {
            if (station.StationType == stationType)
            {
                station.ExitStation();
            }
        }
    }

    private void HandleReturnCameraTweenFinished()
    {
        // Signal to turn off guard clause in Player _PhysicsProcess
        globalSignals.RaisePlayerCanMoveAgain();
    }
}
