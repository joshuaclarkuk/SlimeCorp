using Godot;
using System;

public partial class AnimatedTextComponent : Label
{
    private float percentageIncrement = 0.0f;
    private float elapsedTime = 0.0f;
    private float updateInterval = 0.02f; // Time in seconds between updates
    private bool dialogueIsRevealing = false;

    public override void _Ready()
    {
        VisibleRatio = 0.0f;
    }

    public override void _Process(double delta)
    {
        RevealText(delta);
    }

    private void RevealText(double delta)
    {
        elapsedTime += (float)delta;

        if (elapsedTime >= updateInterval)
        {
            elapsedTime = 0.0f;

            if (VisibleRatio < 1.0f)
            {
                VisibleRatio += percentageIncrement;
            }
            else
            {
                // Stop the process loop when the text is fully revealed
                SetProcess(false);
            }
        }
    }
}
