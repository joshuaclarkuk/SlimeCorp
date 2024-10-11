using Godot;
using System;

public partial class CreatureNeedsDisplay : Control
{
    [Export] private ProgressBar hungerProgressBar = null;
    [Export] private ProgressBar happinessProgressBar = null;
    [Export] private ProgressBar cleanlinessProgressBar = null;
    [Export] private ProgressBar angerProgressBar = null;
    [Export] private Label timeLeftText = null;

    public void UpdateProgressBars(float newHunger, float newHappiness, float newCleanliness, float newAnger, float newTimeLeft)
    {
        hungerProgressBar.Value = newHunger;
        happinessProgressBar.Value = newHappiness;
        cleanlinessProgressBar.Value = newCleanliness;
        angerProgressBar.Value = newAnger;

        // Display time left in minutes (thought could be perceived as hours)
        int minutes = (int)newTimeLeft / 60;
        int seconds = (int)newTimeLeft % 60;

        // Format the string to display as "MM:SS"
        timeLeftText.Text = string.Format("{0}h {1:00}m", minutes, seconds);
    }
}
