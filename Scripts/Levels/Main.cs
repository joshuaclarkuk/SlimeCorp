using Godot;
using System;

public partial class Main : Node3D
{
    [ExportCategory("Titles")]
    [Export] private TitleCard titleCardNode = null;

    [ExportCategory("Creature")]
    [Export] private CreatureNeeds creatureNeeds = null;

    [ExportCategory("Stations")]
    [Export] private Node3D stationsHeaderNode = null;

    [ExportCategory("Music")]
    [Export] private AudioStreamPlayer nonDiegeticMusicNode = null;

    [ExportCategory("Event Nodes")]
    [Export] private Node3D lightingHeaderNode = null;

    [ExportCategory("Computer Items")]
    [Export] private ComputerItemResource[] emailItemResources = null;
    [Export] private ComputerItemResource[] newsItemResources = null;

    // Globals
    private GlobalValues globalValues = null;
    private GlobalSignals globalSignals = null;
    private GlobalEvents globalEvents = null;

    // Day counter
    private int currentDayIndex = 1;
    private int maxDays = 10;

    // Slime counter
    private float slimeCollectedInDay = 0.0f;

    public override void _Ready()
    {
        // Get reference to globals
        globalValues = GetNode<GlobalValues>("/root/GlobalValues");
        globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");
        globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");

        // Connect timeline signals
        globalSignals.OnBlackScreenDisappeared += HandleBlackScreenDisappeared; // Start new day
        globalSignals.OnPlayerClockedIn += HandlePlayerClockedIn;
        globalSignals.OnPlayerClockedOut += HandlePlayerClockedOut; // End day

        // Connect station-based signals
        globalSignals.OnSlimeCanisterRemovedFromStation += HandleSlimeCanisterRemovedFromStation;

        // Send initial stack of emails to computer
        foreach (ComputerItemResource emailResource in emailItemResources)
        {
            globalSignals.RaiseEmailReceived(emailResource);
        }
    }

    public override void _ExitTree()
    {
        // Disconnect timeline signals
        globalSignals.OnBlackScreenDisappeared -= HandleBlackScreenDisappeared;
        globalSignals.OnPlayerClockedIn -= HandlePlayerClockedIn;
        globalSignals.OnPlayerClockedOut -= HandlePlayerClockedOut;

        // Disconnect station-based signals
        globalSignals.OnSlimeCanisterRemovedFromStation -= HandleSlimeCanisterRemovedFromStation;
    }

    private void HandleBlackScreenDisappeared()
    {
        switch (currentDayIndex)
        {
            case 0:
                // Day 0 logic here
                break;
            case 1:
                // Day 1 logic here
                titleCardNode.UpdateTextAndDisplay("Day 1");
                creatureNeeds.SetUpForNewDay(10.0f);
                break;
            case 2:
                // Day 2 logic here
                titleCardNode.UpdateTextAndDisplay("Day 2");
                creatureNeeds.SetUpForNewDay(10.0f);
                break;
            case 3:
                // Day 3 logic here
                titleCardNode.UpdateTextAndDisplay("Day 3");
                creatureNeeds.SetUpForNewDay(10.0f);
                break;
            case 4:
                // Day 4 logic here
                titleCardNode.UpdateTextAndDisplay("Day 4");
                creatureNeeds.SetUpForNewDay(10.0f);
                break;
            case 5:
                // Day 5 logic here
                titleCardNode.UpdateTextAndDisplay("Day 5");
                creatureNeeds.SetUpForNewDay(10.0f);
                break;
            case 6:
                // Day 5 logic here
                titleCardNode.UpdateTextAndDisplay("Day 6");
                creatureNeeds.SetUpForNewDay(10.0f);
                break;
            case 7:
                // Day 5 logic here
                titleCardNode.UpdateTextAndDisplay("Day 7");
                creatureNeeds.SetUpForNewDay(10.0f);
                break;
            case 8:
                // Day 5 logic here
                titleCardNode.UpdateTextAndDisplay("Day 8");
                creatureNeeds.SetUpForNewDay(10.0f);
                break;
            case 9:
                // Day 5 logic here
                titleCardNode.UpdateTextAndDisplay("Day 9");
                creatureNeeds.SetUpForNewDay(10.0f);
                break;
            case 10:
                // Day 5 logic here
                titleCardNode.UpdateTextAndDisplay("Day 10");
                creatureNeeds.SetUpForNewDay(10.0f);
                break;
        }
    }

    private void HandlePlayerClockedIn()
    {
        // Start music
        nonDiegeticMusicNode.Play();
        globalEvents.RaiseMainKillLightsEvent(lightingHeaderNode);
    }

    private void HandlePlayerClockedOut()
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
