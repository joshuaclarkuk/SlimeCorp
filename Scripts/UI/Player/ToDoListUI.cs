using Godot;
using System;

public partial class ToDoListUI : VBoxContainer
{
    [ExportCategory("Behaviour")]
    [Export(PropertyHint.Range, "0.01,0.05")] private float textRevealPercentageIncrement = 0.03f;

    private GlobalSignals globalSignals;

    public override void _Ready()
    {
        globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");
        globalSignals.OnBlackScreenDisappeared += HandleBlackScreenDisappeared;
        globalSignals.OnPlayerClockedIn += HandlePlayerClockedIn;

        Visible = false;
    }

    public override void _ExitTree()
    {
        globalSignals.OnBlackScreenDisappeared -= HandleBlackScreenDisappeared;
        globalSignals.OnPlayerClockedIn -= HandlePlayerClockedIn;
    }

    private void HandleBlackScreenDisappeared()
    {
        //Visible = true;

        foreach (Node child in GetChildren())
        {
            if (child is AnimatedText animatedText)
            {
                animatedText.StartAnimatingText(textRevealPercentageIncrement);
            }
        }
    }

    private void HandlePlayerClockedIn()
    {
        Visible = false;

        foreach (Node child in GetChildren())
        {
            if (child is AnimatedText animatedText)
            {
                animatedText.ResetText();
            }
        }
    }
}
