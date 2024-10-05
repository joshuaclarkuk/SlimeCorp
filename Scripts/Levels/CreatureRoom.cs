using Godot;
using System;

public partial class CreatureRoom : Node3D
{
    [ExportCategory("Titles")]
    [Export] private TitleCard titleCardNode = null;

    [ExportCategory("Creature")]
    [Export] private CreatureNeeds creatureNeeds = null;

    [ExportCategory("Stations")]
    [Export] private Node3D stationsHeaderNode = null;

    [ExportCategory("Computer Items")]
    [Export] private ComputerItemResource[] emailItemResources = null;
    [Export] private ComputerItemResource[] newsItemResources = null;

    // Signals
    private GlobalSignals globalSignals;

    // Day counter
    private int currentDayIndex = 0;
    private int maxDays = 5;

    // Slime counter
    private float slimeCollectedInDay = 0.0f;

    public override void _Ready()
    {
        globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");

        // Connect timeline signals
        globalSignals.OnStartNewDay += HandleOnStartNewDay;
        globalSignals.OnEndDay += HandleOnEndDay;
        globalSignals.OnBlackScreenDisappeared += HandleBlackScreenDisappeared;
        // Connect station-based signals
        globalSignals.OnSlimeCanisterRemovedFromStation += HandleSlimeCanisterRemovedFromStation;

        // Start at day zero
        globalSignals.RaiseStartNewDay(currentDayIndex); // index should be zero

        // Send initial stack of emails to computer
        foreach (ComputerItemResource emailResource in emailItemResources)
        {
            globalSignals.RaiseEmailReceived(emailResource);
        }
    }

    public override void _ExitTree()
    {
        // Disconnect timeline signals
        globalSignals.OnStartNewDay -= HandleOnStartNewDay;
        globalSignals.OnEndDay -= HandleOnEndDay;
        // Disconnect station-based signals
        globalSignals.OnSlimeCanisterRemovedFromStation -= HandleSlimeCanisterRemovedFromStation;
    }

    private void HandleOnStartNewDay(int dayIndex)
    {
        switch (dayIndex)
        {
            case 0:
                // Day 0 logic here
                creatureNeeds.SetUpForNewDay(10.0f); // Replace with resource when created
                GenerateEmployeeNumber();
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

    private void HandleBlackScreenDisappeared()
    {
        switch (currentDayIndex)
        {
            case 0:
                // Day 0 logic here
                creatureNeeds.SetUpForNewDay(10.0f); // Replace with resource when created
                GenerateEmployeeNumber();
                titleCardNode.UpdateTextAndDisplay("Orientation");
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

    private void ResetAllCreatureNeeds()
    {
        // Reset all creature needs
        // Set number of minutes in day
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

    private void GenerateEmployeeNumber()
    {
        // Generate employee number here
        Random random = new Random();
        int[] employeeNumber = new int[4];
        for (int i = 0; i < employeeNumber.Length; i++)
        {
            employeeNumber[i] = random.Next(0, 10);
        }

        // Pass code in to event
        globalSignals.RaiseEmployeeNumberGenerated(employeeNumber);
    }

    private void HandleSlimeCanisterRemovedFromStation(float slimeAmount)
    {
        slimeCollectedInDay += slimeAmount;
        GD.Print($"MAIN: Banked {slimeAmount} slime. Daily total: {slimeCollectedInDay}");
    }

    private void EndGame()
    {
        // Initiate final sequence
    }
}
