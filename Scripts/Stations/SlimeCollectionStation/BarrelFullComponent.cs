using Godot;
using System;

public partial class BarrelFullComponent : Node3D
{
    [ExportCategory("Required Nodes")]
    [Export] private OmniLight3D barrelFullLightNode = null;
    [Export] private AudioStreamPlayer3D barrelFullSoundNode = null;

    private float flashTime = 0.1f;
    private float invisibleTime = 0.9f;

    private float currentFlashTime = 0.0f;
    private float currentInvisibleTime = 0.0f;

    private bool isFlashing = false;

    public override void _Ready()
    {
        barrelFullLightNode.Visible = false;
        barrelFullSoundNode.Stop();

        SetProcess(false);
    }

    public override void _Process(double delta)
    {
        if (currentFlashTime > 0)
        {           
            currentFlashTime -= (float)delta;
            if (currentFlashTime <= 0)
            {
                // Turn light off and start invisible timer
                barrelFullLightNode.Visible = false;
                currentInvisibleTime = invisibleTime;
            }
        }
        else if (currentInvisibleTime > 0)
        {
            // Light is off, decrement invisible timer
            currentInvisibleTime -= (float)delta;
            if (currentInvisibleTime <= 0)
            {
                // Time to flash again
                barrelFullLightNode.Visible = true;
                currentFlashTime = flashTime;
            }
        }
    }

    public void BarrelFull()
    {
        if (!isFlashing)
        {
            barrelFullSoundNode?.Play(0.5f);
            currentFlashTime = flashTime;
            currentInvisibleTime = invisibleTime;
            SetProcess(true);
            isFlashing = true;
        }

    }

    public void FullBarrelRemoved()
    {
        if (isFlashing)
        {
            barrelFullSoundNode.Stop();
            barrelFullLightNode.Visible = true;
            SetProcess(false);
            isFlashing = false;
        }
    }
}
