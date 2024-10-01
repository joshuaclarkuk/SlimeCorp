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
        float percentageStep = mouseDragMotion.Y * (range * mouseDragSensitivity);

        if (startRotation > targetRotation)
        {
            currentRotation += percentageStep;
            currentRotation = Mathf.Clamp(currentRotation, startRotation, targetRotation);
        }
        else
        {
            currentRotation -= percentageStep;
            currentRotation = Mathf.Clamp(currentRotation, startRotation, targetRotation);
        }

        UpdateRotation(currentRotation);

        if (currentRotation >= targetRotation * 0.9f)
        {
            GD.Print("Lever at end of motion");
        }
    }

    private void UpdateRotation(float newRotation)
    {
        Rotation = new Vector3(newRotation, Rotation.Y, Rotation.Z);

        float ninetyPercentPoint = 0.0f;
        if (startRotation > targetRotation)
        {
            ninetyPercentPoint = targetRotation + (range * 0.1f);  // 90% towards the target (downwards)
            if (currentRotation <= ninetyPercentPoint)
            {
                OnLeverTargetReached?.Invoke();
            }
            else
            {
                OnLeverTargetLeft?.Invoke();
            }
        }
        else
        {
            ninetyPercentPoint = targetRotation - (range * 0.1f);  // 90% towards the target (upwards)
            if (currentRotation >= ninetyPercentPoint)
            {
                OnLeverTargetReached?.Invoke();
            }
            else
            {
                OnLeverTargetLeft?.Invoke();
            }
        }
    }
}
