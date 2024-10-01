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

    public void MovePhysicalCardWithMouseMotion(float mouseDragMotion, float mouseDragSensitivity)
    {
        float percentageStep = mouseDragMotion * (range * mouseDragSensitivity);

        // If startLocation is greater that targetLocation, clamp on the negative, otherwise clamp on the positive
        if (startLocation > targetLocation)
        {
            currentLocation += percentageStep;
            currentLocation = Mathf.Clamp(currentLocation, targetLocation, startLocation);
        }
        else
        {
            currentLocation -= percentageStep;
            currentLocation = Mathf.Clamp(currentLocation, startLocation, targetLocation);
        }

        UpdateLocation(currentLocation);
    }

    public void ReturnToOriginalPosition()
    {
        UpdateLocation(startLocation);
        currentLocation = startLocation;
    }

    private void UpdateLocation(float newLocation)
    {
        Position = new Vector3(Position.X, newLocation, Position.Z);

        float ninetyPercentPoint = 0.0f;
        if (startLocation > targetLocation)
        {
            ninetyPercentPoint = targetLocation + (range * 0.1f);  // 90% towards the target (downwards)
            if (currentLocation <= ninetyPercentPoint)
            {
                OnCardTargetReached?.Invoke();
            }
            else
            {
                OnCardTargetLeft?.Invoke();
            }
        }
        else
        {
            ninetyPercentPoint = targetLocation - (range * 0.1f);  // 90% towards the target (upwards)
            if (currentLocation >= ninetyPercentPoint)
            {
                OnCardTargetReached?.Invoke();
            }
            else
            {
                OnCardTargetLeft?.Invoke();
            }
        }
    }
}
