using Godot;
using System;

public partial class CreatureNeedsDisplay : Control
{
    [Export] private ProgressBar hungerProgressBar = null;
    [Export] private ProgressBar happinessProgressBar = null;
    [Export] private ProgressBar cleanlinessProgressBar = null;
    [Export] private Label timeLeftText = null;

    public void UpdateProgressBars(float newHunger, float newHappiness, float newCleanliness, float newTimeLeft)
    {
        hungerProgressBar.Value = newHunger;
        happinessProgressBar.Value = newHappiness;
        cleanlinessProgressBar.Value = newCleanliness;
        timeLeftText.Text = Mathf.RoundToInt(newTimeLeft).ToString();
    }
}
