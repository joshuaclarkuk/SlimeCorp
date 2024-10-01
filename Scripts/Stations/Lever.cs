using Godot;
using System;

public partial class Lever : Node3D
{
    [ExportCategory("Behaviour")]
    [Export] private float desiredStartRotation = -45.0f;
    [Export] private float desiredTargetRotation = 45.0f;

    private float startRotation = Mathf.DegToRad(-45.0f);
    private float targetRotation = Mathf.DegToRad(45.0f);
    private float currentRotation = 0.0f;

    public event Action OnLeverTargetReached;
    public event Action OnLeverTargetLeft;

    public override void _Ready()
    {
        // Can't export DegToRad values due to confusing maths, so need to initialise instead
        startRotation = Mathf.DegToRad(desiredStartRotation);
        targetRotation = Mathf.DegToRad(desiredTargetRotation);
        currentRotation = startRotation;

        // Make sure lever starts in correct position
        Rotation = new Vector3(currentRotation, Rotation.Y, Rotation.Z);
    }

    // Be able to drag a lever down using mouse drag
    public void MovePhysicalHandleWithMouseMotion(float mouseDragSensitivity, Vector2 mouseDragMotion)
    {
        float range = startRotation - targetRotation;
        float percentageStep = (range * mouseDragSensitivity) * mouseDragMotion.Y;
        currentRotation += percentageStep;
        currentRotation = Mathf.Clamp(currentRotation, startRotation, targetRotation);

        Rotation = new Vector3(currentRotation, Rotation.Y, Rotation.Z);

        if (currentRotation >= targetRotation * 0.9f)
        {
            GD.Print("Lever at end of motion");
        }
    }
}
