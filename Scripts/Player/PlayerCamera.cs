using Godot;
using System;

public partial class PlayerCamera : Camera3D
{
    [ExportCategory("Smooth Camera")]
    [Export] private float snapSpeed = 44.0f;

    public override void _PhysicsProcess(double delta)
    {
        float weight = Mathf.Clamp((float)delta * snapSpeed, 0.0f, 1.0f);

        GlobalTransform = GlobalTransform.InterpolateWith(GetParent<Node3D>().GlobalTransform, weight);

        GlobalPosition = GetParent<Node3D>().GlobalPosition;
    }
}
