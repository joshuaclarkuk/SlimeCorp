using Godot;
using System;

public partial class Main : Node3D
{
    [ExportCategory("Resources")]
    [Export] private ArticleResource[] articleResources;
    [Export] private EmailResource[] emailResources;

    // Signals
    private GlobalSignals globalSignals;

    // Day counter
    private int currentDayIndex = 0;
    private int maxDays = 5;

    public override void _Ready()
    {
        // Connect signals
        globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");
        globalSignals.OnStartNewDay += HandleOnStartNewDay;
        globalSignals.OnEndDay += HandleOnEndDay;

        // Resource debug error checks
        if (articleResources.Length < maxDays) { GD.PrintErr("Not all article resources present!"); }
        if (emailResources.Length < maxDays) { GD.PrintErr("Not all email resources present!"); }

        // Start at day zero
        globalSignals.RaiseStartNewDay(currentDayIndex); // index should be zero
    }

    private void HandleOnStartNewDay(int dayIndex)
    {
        switch (dayIndex)
        {
            case 0:
                // Day 0 logic here
                break;
            case 1:
                // Day 1 logic here
                break;
            case 2:
                // Day 2 logic here
                break;
            case 3:
                // Day 3 logic here
                break;
            case 4:
                // Day 4 logic here
                break;
            case 5:
                // Day 5 logic here
                break;
        }
    }

    private void HandleOnEndDay()
    {
        // End day logic here
        currentDayIndex++;
    }

    private void ResetAllCreatureNeeds()
    {
        // Reset all creature needs
    }

    private void AddNewsArticleToComputer(int articleIndex)
    {
        // Add news item to computer
        // Pass in news article resource
    }

    private void AddEmailToComputer(int emailIndex)
    {
        // Add email to computer
        // Pass in email resource
    }

    private void ResetSlimeCollectedAmount()
    {
        // Set slime collected back to zero
    }

    private void EndGame()
    {

    }
}
