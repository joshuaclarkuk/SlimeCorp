using Godot;
using System;

public partial class CreatureNeeds : Node3D
{
    [ExportCategory("Required Nodes")]
    [Export] private CreatureNeedsDisplay creatureNeedsDisplayNode = null;
    [Export] private Timer needsDisplayUpdateTimerNode = null;

    [ExportCategory("Behaviour")]
    [Export] private float hungerDepletionRate = 1.0f;
    [Export] private float happinessDepletionRate = 1.0f;
    [Export] private float cleanlinessDepletionRate = 1.0f;

    private GlobalSignals globalSignals = null;

    private float maxHungerLevel = 100.0f;
    private float maxHappinessLevel = 100.0f;
    private float maxCleanlinessLevel = 100.0f;

    private float currentHungerLevel = 0.0f;
    private float currentHappinessLevel = 0.0f;
    private float currentCleanlinessLevel = 0.0f;
    private float currentTimeLeft = 0.0f;

    private bool playerHasClockedIn = false;

    public override void _Ready()
    {
        // Link global signals so we know when player has clocked in and out
        globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");
        globalSignals.OnPlayerClockedIn += HandlePlayerClockedIn;
        globalSignals.OnPlayerClockedOut += HandlePlayerClockedOut;

        // Link timer signal to update UI on timeout
        needsDisplayUpdateTimerNode.Timeout += HandleNeedsDisplayUpdateTimerNodeTimeout;

        ResetAllCreatureNeeds();
        creatureNeedsDisplayNode.UpdateProgressBars(currentHungerLevel, currentHappinessLevel, currentCleanlinessLevel, currentTimeLeft);
        SetNumberOfMinutesInDay(1.0f); // DELETE THIS WHEN MAIN SCRIPT IS HANDLING DAY INITIATION
    }

    public override void _ExitTree()
    {
        needsDisplayUpdateTimerNode.Timeout -= HandleNeedsDisplayUpdateTimerNodeTimeout;
        globalSignals.OnPlayerClockedIn -= HandlePlayerClockedIn;
        globalSignals.OnPlayerClockedOut -= HandlePlayerClockedOut;
    }

    public override void _Process(double delta)
    {
        if (playerHasClockedIn)
        {
            currentHungerLevel -= (float)delta * hungerDepletionRate;
            currentHappinessLevel -= (float)delta * happinessDepletionRate;
            currentCleanlinessLevel -= (float)delta * cleanlinessDepletionRate;
            currentTimeLeft -= (float)delta;
        }
    }

    public void ResetAllCreatureNeeds()
    {
        currentHungerLevel = maxHungerLevel;
        currentHappinessLevel = maxHappinessLevel;
        currentCleanlinessLevel = maxCleanlinessLevel;
    }

    public void SetNumberOfMinutesInDay(float minutes)
    {
        currentTimeLeft = minutes * 60.0f;
    }

    private void HandleNeedsDisplayUpdateTimerNodeTimeout()
    {
        float newHungerPercentage = currentHungerLevel / maxHungerLevel * 100.0f;
        float newHappinessPercentage = currentHappinessLevel / maxHappinessLevel * 100.0f;
        float newCleanlinessPercentage = currentCleanlinessLevel / maxCleanlinessLevel * 100.0f;
        creatureNeedsDisplayNode.UpdateProgressBars(newHungerPercentage, newHappinessPercentage, newHappinessPercentage, currentTimeLeft);
    }

    private void HandlePlayerClockedIn()
    {
        playerHasClockedIn = true;
        needsDisplayUpdateTimerNode.Start();
    }

    private void HandlePlayerClockedOut()
    {
        playerHasClockedIn = false;
        needsDisplayUpdateTimerNode.Stop();
    }
}
