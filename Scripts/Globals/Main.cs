using Godot;
using System;

public partial class Main : Node3D
{
    [ExportCategory("Creature")]
    [Export] private CreatureNeeds creatureNeeds = null;

    [ExportCategory("Resources")]
    [Export] private ArticleResource[] articleResources;
    [Export] private EmailResource[] emailResources;

    [ExportCategory("Stations")]
    [Export] private Node3D stationsHeaderNode = null;

    // Signals
    private GlobalSignals globalSignals;

    // Day counter
    private int currentDayIndex = 0;
    private int maxDays = 5;

    public override void _Ready()
    {
        globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");

        // Connect timeline signals
        globalSignals.OnStartNewDay += HandleOnStartNewDay;
        globalSignals.OnEndDay += HandleOnEndDay;
        // Connect station-based signals
        globalSignals.OnPlayerInteractWithStation += HandlePlayerInteractWithStation;
        globalSignals.OnPlayerExitStation += HandlePlayerExitStation;


        // Resource debug error checks
        if (articleResources.Length < maxDays) { GD.PrintErr("Not all article resources present!"); }
        if (emailResources.Length < maxDays) { GD.PrintErr("Not all email resources present!"); }

        // Start at day zero
        globalSignals.RaiseStartNewDay(currentDayIndex); // index should be zero
    }

    public override void _ExitTree()
    {
        // Disconnect timeline signals
        globalSignals.OnStartNewDay -= HandleOnStartNewDay;
        globalSignals.OnEndDay -= HandleOnEndDay;
        // Disconnect station-based signals
        globalSignals.OnPlayerInteractWithStation -= HandlePlayerInteractWithStation;
        globalSignals.OnPlayerExitStation -= HandlePlayerExitStation;
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

    private void HandlePlayerInteractWithStation(E_StationType type)
    {
        
    }

    private void HandlePlayerExitStation(E_StationType type)
    {
        
    }

    private void EndGame()
    {
        // Initiate final sequence
    }
}
