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
    private float range = 0.0f;

    public event Action OnLeverTargetReached;
    public event Action OnLeverTargetLeft;

    public override void _Ready()
    {
        // Can't export DegToRad values due to confusing maths, so need to initialise instead
        startRotation = Mathf.DegToRad(desiredStartRotation);
        targetRotation = Mathf.DegToRad(desiredTargetRotation);
        currentRotation = startRotation;

        range = Mathf.Abs(startRotation - targetRotation);

        // Make sure lever starts in correct position
        UpdateRotation(currentRotation);
    }

    // Be able to drag a lever down using mouse drag
    public void MovePhysicalHandleWithMouseMotion(float mouseDragSensitivity, Vector2 mouseDragMotion)
    {
        if (startRotation > targetRotation)
        {
            float percentageStep = -mouseDragMotion.Y * (range * mouseDragSensitivity);
            currentRotation += percentageStep;
        }
        else
        {
            float percentageStep = mouseDragMotion.Y * (range * mouseDragSensitivity);
            currentRotation -= percentageStep;
        }

        // Always clamps rotation no matter whether the lever starts in the up position or the down position
        currentRotation = Mathf.Clamp(currentRotation, Mathf.Min(startRotation, targetRotation), Mathf.Max(startRotation, targetRotation));

        UpdateRotation(currentRotation);
    }

    private void UpdateRotation(float newRotation)
    {
        Rotation = new Vector3(newRotation, Rotation.Y, Rotation.Z);

        // Always calculates correct threshold no matter whether the lever starts in the up position or the down position
        if (Math.Abs(currentRotation - targetRotation) <= 0.1f * range)
        {
            GD.Print("Lever at end of motion");
            OnLeverTargetReached?.Invoke();
        }
        else
        {
            OnLeverTargetLeft?.Invoke();
        }
    }
}
