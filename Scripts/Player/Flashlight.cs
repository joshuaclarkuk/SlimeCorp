using Godot;
using System;

public partial class Flashlight : Node3D
{
    [ExportCategory("Required Nodes")]
    [Export] private SpotLight3D lightNode = null;

    public override void _Ready()
    {
        lightNode.Visible = false;
    }

    public void InterpLightWithCamera(double delta, Camera3D playerCamera)
    {
        GlobalTransform = GlobalTransform.InterpolateWith(playerCamera.GlobalTransform, (float)delta * 10.0f);
    }

    public void ToggleFlashlight(bool shouldBeActive)
    {
        if (shouldBeActive)
        {
            lightNode.Visible = true;
        }
        else
        {
            lightNode.Visible = false;
        }
    }
}
