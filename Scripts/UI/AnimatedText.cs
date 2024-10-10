using Godot;
using System;

public partial class AnimatedText : Label
{
    [ExportCategory("Text To Reveal")]
    [Export(PropertyHint.MultilineText)] private string textToReveal = string.Empty;

    private float percentageIncrement = 0.01f;
    private float elapsedTime = 0.0f;
    private float updateInterval = 0.02f; // Time in seconds between updates

    public override void _Ready()
    {
        Text = textToReveal;
        VisibleRatio = 0.0f;
        SetProcess(false);
    }

    public override void _Process(double delta)
    {
        RevealText(delta);
    }

    public void StartAnimatingText(float percentageIncrement)
    {
        this.percentageIncrement = Mathf.Min(percentageIncrement, 0.05f);
        SetProcess(true);
    }

    public void ResetText()
    {
        VisibleRatio = 0.0f;
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

                if (VisibleRatio > 1.0f)
                {
                    VisibleRatio = 1.0f;  // Clamp the value to 1.0
                }
            }
            else
            {
                // Stop the process loop when the text is fully revealed
                SetProcess(false);
            }
        }
    }
}
