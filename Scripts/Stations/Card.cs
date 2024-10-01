using Godot;
using System;

public partial class Card : Node3D
{
    [ExportCategory("Behaviour")]
    [Export] private float startLocation = 0.0f;
    [Export] private float targetLocation = 0.0f;
    private float currentLocation = 0.0f;
    private float range = 0.0f;

    public event Action OnCardTargetReached;
    public event Action OnCardTargetLeft;

    public override void _Ready()
    {
        currentLocation = startLocation;
        range = Mathf.Abs(startLocation - targetLocation);
        UpdateLocation(currentLocation);
    }

    public void ReturnToOriginalPosition()
    {
        UpdateLocation(startLocation);
        currentLocation = startLocation;
    }

    public void MovePhysicalCardWithMouseMotion(float mouseDragMotion, float mouseDragSensitivity)
    {
        // If startLocation is greater that targetLocation, clamp on the negative, otherwise clamp on the positive
        if (startLocation > targetLocation)
        {
            float percentageStep = mouseDragMotion * (range * mouseDragSensitivity);
            currentLocation += percentageStep;
        }
        else
        {
            float percentageStep = -mouseDragMotion * (range * mouseDragSensitivity);
            currentLocation -= percentageStep;
        }

        // Always clamps location no matter whether the card needs swiping up or down
        currentLocation = Mathf.Clamp(currentLocation, Mathf.Min(startLocation, targetLocation), Mathf.Max(startLocation, targetLocation));

        UpdateLocation(currentLocation);
    }

    private void UpdateLocation(float newLocation)
    {
        Position = new Vector3(Position.X, newLocation, Position.Z);

        if (Mathf.Abs(currentLocation - targetLocation) <= 0.1f * range)
        {
            OnCardTargetReached?.Invoke();
        }
        else
        {
            OnCardTargetLeft?.Invoke();
        }
    }
}
