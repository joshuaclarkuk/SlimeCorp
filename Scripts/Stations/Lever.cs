using Godot;
using System;

public partial class Lever : Node3D
{
    [ExportCategory("Behaviour")]
    [Export] private float startRotation = Mathf.DegToRad(-45.0f);
    [Export] private float targetRotation = Mathf.DegToRad(45.0f);

    private float currentRotation = 0.0f;

    public override void _Ready()
    {
        currentRotation = startRotation;
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
