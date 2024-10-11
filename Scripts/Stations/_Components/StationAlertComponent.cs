using Godot;
using System;

public partial class StationAlertComponent : Node3D
{
    [ExportCategory("Light and Sound")]
    [Export] private Color lightColor;
    [Export] private AudioStream soundToPlay = null;

    [ExportCategory("Interior Nodes")]
    [Export] private OmniLight3D lightNode = null;
    [Export] private AudioStreamPlayer3D soundNode = null;

    private float flashTime = 0.1f;
    private float invisibleTime = 0.9f;

    private float currentFlashTime = 0.0f;
    private float currentInvisibleTime = 0.0f;

    private bool isFlashing = false;

    public override void _Ready()
    {
        soundNode.Stream = soundToPlay;
        lightNode.LightColor = lightColor;
        lightNode.Visible = false;

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
                lightNode.Visible = false;
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
                lightNode.Visible = true;
                soundNode.Play();
                currentFlashTime = flashTime;
            }
        }
    }

    public void TriggerStationAlert()
    {
        if (!isFlashing)
        {
            currentFlashTime = flashTime;
            currentInvisibleTime = invisibleTime;
            SetProcess(true);
            isFlashing = true;
        }

    }

    public void SilenceStationAlert()
    {
        if (isFlashing)
        {
            lightNode.Visible = false;
            SetProcess(false);
            isFlashing = false;
        }
    }
}
