using Godot;
using System;

public partial class Valve : Node3D
{
    [ExportCategory("Behaviour")]
    [Export] private float desiredStartRotation = 0.0f;
    [Export] private float desiredTargetRotation = -360.0f;

    private float startRotation = Mathf.DegToRad(0.0f);
    private float targetRotation = Mathf.DegToRad(-180.0f);
    private float currentRotation = 0.0f;
    private float range = 0.0f;

    private bool isValveOpen = false;
    private bool isInTargetZone = false; // Prevents multiple firings on the event

    public event Action<bool> OnValveTargetReached; // bool values are true = "Valve Open" and false = "Valve Closed"

    public override void _Ready()
    {
        // Can't export DegToRad values due to confusing maths, so need to initialise instead
        startRotation = Mathf.DegToRad(desiredStartRotation);
        targetRotation = Mathf.DegToRad(desiredTargetRotation);
        currentRotation = startRotation;

        range = Mathf.Abs(startRotation - targetRotation);

        // Make sure valve starts in correct position
        UpdateRotation(currentRotation);
    }

    // Be able to drag a lever down using mouse drag
    public void MovePhysicalValveWithMouseMotion(float mouseDragSensitivity, Vector2 mouseDragMotion)
    {
        if (!isValveOpen)
        {
            float percentageStep = -mouseDragMotion.Y * (range * mouseDragSensitivity);
            currentRotation -= percentageStep;
        }
        else
        {
            float percentageStep = mouseDragMotion.Y * (range * mouseDragSensitivity);
            currentRotation += percentageStep;
        }

        // Always clamps rotation no matter whether the lever starts in the up position or the down position
        currentRotation = Mathf.Clamp(currentRotation, Mathf.Min(startRotation, targetRotation), Mathf.Max(startRotation, targetRotation));
        UpdateRotation(currentRotation);
    }

    // FEELS LIKE THERE'S SOME REDUNDANT CODE IN HERE BUT IT'S WORKING SO I'M JUST NOT GOING TO TOUCH IT
    private void UpdateRotation(float newRotation)
    {
        Rotation = new Vector3(Rotation.X, Rotation.Y, newRotation);

        float threshold = 0.9f * range;

        // Always calculates correct threshold no matter whether the valve is open or closed
        if (Mathf.Abs(currentRotation - targetRotation) <= threshold)
        {
            if (isInTargetZone)
            {
                isInTargetZone = true;
            }
        }
        else
        {
            if (!isInTargetZone)
            {
                OnValveTargetReached?.Invoke(isValveOpen);

                isValveOpen = !isValveOpen; // Toggle open and closed
                SwapValvePolarity();
                isInTargetZone = false;
            }
        }
    }

    private void SwapValvePolarity()
    {
        float tempRotation = startRotation;
        startRotation = targetRotation;
        targetRotation = tempRotation;

        // Recalculate the range based on new start and target positions
        range = Mathf.Abs(startRotation - targetRotation);
    }
}
