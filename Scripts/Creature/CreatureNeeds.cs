using Godot;
using System;

public partial class CreatureNeeds : Node3D
{
    [ExportCategory("Required Nodes")]
    [Export] private FeedingComponent feedingComponentNode = null;
    [Export] private CleaningComponent cleaningComponentNode = null;
    [Export] private CreatureNeedsDisplay creatureNeedsDisplayNode = null;
    [Export] private Timer needsDisplayUpdateTimerNode = null;
    [Export] private DebugUI debugUINode = null;

    [ExportCategory("Depletion Rates")]
    [Export] private float hungerDepletionRate = 0.6f;
    [Export] private float happinessDepletionRate = 0.4f;
    [Export] private float cleanlinessDepletionRate = 0.5f;

    [ExportCategory("Request Points (Percentage Left)")]
    [Export] private float percentageMaxHungerBeforeFeedingRequestMade = 95.0f;
    [Export] private float percentageMaxCleanlinessBeforeCleaningRequestMade = 95.0f;

    [ExportCategory("Replenishment/Addition Rates")]
    [Export] private float maxHungerReplenishment = 100.0f;
    [Export] private float maxCleanlinessReplenishment = 75.0f;
    [Export] private float maxWasteProductToAdd = 50.0f;
    [Export] private float maxAngerToAdd = 10.0f;

    private GlobalSignals globalSignals = null;

    // Max levels
    private float maxHungerLevel = 100.0f;
    private float maxHappinessLevel = 100.0f;
    private float maxCleanlinessLevel = 100.0f;

    // Current levels
    private float currentHungerLevel = 0.0f;
    private float currentHappinessLevel = 0.0f;
    private float currentCleanlinessLevel = 0.0f;
    private float currentTimeLeft = 0.0f;

    // Guards
    private bool playerHasClockedIn = false;
    private bool hasMadeFeedingRequest = false;
    private bool hasMadeCleaningRequest = false;
    private bool isFeedingRequestSatisfied = false;
    private bool isCleaningRequestSatisfied = false;

    public override void _Ready()
    {
        // Link global signals so we know when player has clocked in and out
        globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");
        globalSignals.OnPlayerClockedIn += HandlePlayerClockedIn;
        globalSignals.OnPlayerClockedOut += HandlePlayerClockedOut;

        // Link timer signal to update UI on timeout
        needsDisplayUpdateTimerNode.Timeout += HandleNeedsDisplayUpdateTimerNodeTimeout;

        // Link NeedComponent signals
        feedingComponentNode.OnCreatureServedFood += HandleCreatureServedFood;
        cleaningComponentNode.OnAreaCleaned += HandleAreaCleaned;
    }

    public override void _ExitTree()
    {
        needsDisplayUpdateTimerNode.Timeout -= HandleNeedsDisplayUpdateTimerNodeTimeout;
        globalSignals.OnPlayerClockedIn -= HandlePlayerClockedIn;
        globalSignals.OnPlayerClockedOut -= HandlePlayerClockedOut;

        // Unlink NeedComponent signals
        feedingComponentNode.OnCreatureServedFood -= HandleCreatureServedFood;
        cleaningComponentNode.OnAreaCleaned -= HandleAreaCleaned;
    }

    public override void _Process(double delta)
    {
        if (playerHasClockedIn)
        {
            ReduceNeedLevels(delta);
        }
    }

    // Called by Main to initialise needs per day
    public void SetUpForNewDay(float minutesInDay)
    {
        SetNumberOfMinutesInDay(minutesInDay);
        ResetAllCreatureNeeds();
        creatureNeedsDisplayNode.UpdateProgressBars(currentHungerLevel, currentHappinessLevel, currentCleanlinessLevel, currentTimeLeft);
        debugUINode.UpdateProgressBars(currentHungerLevel, currentHappinessLevel, currentCleanlinessLevel, currentTimeLeft);
    }

    private void SetNumberOfMinutesInDay(float minutes)
    {
        currentTimeLeft = minutes * 60.0f;
    }

    private void ResetAllCreatureNeeds()
    {
        currentHungerLevel = maxHungerLevel;
        currentHappinessLevel = maxHappinessLevel;
        currentCleanlinessLevel = maxCleanlinessLevel;
    }

    private void ReduceNeedLevels(double delta)
    {
        currentHungerLevel = Mathf.Max(0, currentHungerLevel - (float)delta * hungerDepletionRate);
        currentHappinessLevel = Mathf.Max(0, currentHappinessLevel - (float)delta * happinessDepletionRate);
        currentCleanlinessLevel = Mathf.Max(0, currentCleanlinessLevel - (float)delta * cleanlinessDepletionRate);
        currentTimeLeft = Mathf.Max(0, currentTimeLeft - (float)delta);

        MakeFeedingRequestIfBelowThreshold();
        MakeCleaningRequestIfBelowThreshold();
    }

    private void AddWasteProduct(float amountOfWaste)
    {
        currentCleanlinessLevel -= amountOfWaste;
    }

    private void MakeFeedingRequestIfBelowThreshold()
    {
        if (currentHungerLevel <= maxHungerLevel * (percentageMaxHungerBeforeFeedingRequestMade / 100.0f) && !hasMadeFeedingRequest)
        {
            GD.Print("Hunger below threshold. Making feeding request");
            feedingComponentNode.ProcessFeedingRequest();
            hasMadeFeedingRequest = true;
            isFeedingRequestSatisfied = false;
        }
    }

    private void MakeCleaningRequestIfBelowThreshold()
    {
        if (currentCleanlinessLevel <= maxCleanlinessLevel * (percentageMaxCleanlinessBeforeCleaningRequestMade / 100.0f) && !hasMadeCleaningRequest)
        {
            GD.Print("Cleanliness below threshold. Making cleaning request");
            cleaningComponentNode.ProcessCleaningRequest();
            hasMadeCleaningRequest = true;
            isCleaningRequestSatisfied = false;
        }
    }

    private void HandleNeedsDisplayUpdateTimerNodeTimeout()
    {
        float newHungerPercentage = currentHungerLevel / maxHungerLevel * 100.0f;
        float newHappinessPercentage = currentHappinessLevel / maxHappinessLevel * 100.0f;
        float newCleanlinessPercentage = currentCleanlinessLevel / maxCleanlinessLevel * 100.0f;
        creatureNeedsDisplayNode.UpdateProgressBars(newHungerPercentage, newHappinessPercentage, newHappinessPercentage, currentTimeLeft);
        debugUINode.UpdateProgressBars(currentHungerLevel, currentHappinessLevel, currentCleanlinessLevel, currentTimeLeft); // DEBUG TO REMOVE
    }

    private void HandleCreatureServedFood(E_NeedMetAmount amount)
    {
        float newHungerLevel = 0.0f;

        switch (amount)
        {
            case E_NeedMetAmount.NONE:
                // Add anger mutiplied by 1.0
                break;
            case E_NeedMetAmount.HALF:
                newHungerLevel = currentHungerLevel + maxHungerReplenishment * 0.5f;
                currentHungerLevel = Mathf.Min(newHungerLevel, maxHungerLevel * 1.2f); // Cap at 20% over max
                AddWasteProduct(maxWasteProductToAdd * 0.5f);
                // Add anger mutiplied by 0.5
                break;
            case E_NeedMetAmount.MOST:
                newHungerLevel = currentHungerLevel + maxHungerReplenishment * 0.75f;
                currentHungerLevel = Mathf.Min(newHungerLevel, maxHungerLevel * 1.2f); // Cap at 20% over max
                AddWasteProduct(maxWasteProductToAdd * 0.75f);
                // Add anger mutiplied by 0.25
                break;
            case E_NeedMetAmount.ALL:
                newHungerLevel = currentHungerLevel + maxHungerReplenishment * 1.0f;
                currentHungerLevel = Mathf.Min(newHungerLevel, maxHungerLevel * 1.2f); // Cap at 20% over max
                AddWasteProduct(maxWasteProductToAdd * 1.0f);
                // For every five points over 100, add an additional waste product penalty
                break;
            default:
                GD.PrintErr("No valid food amount received by CreatureNeeds from FeedingComponent");
                break;
        }

        // Add excess waste if OVERFEEDING creature
        if (currentHungerLevel > maxHungerLevel)
        {
            float excessHunger = currentHungerLevel - maxHungerLevel;
            int additionalWaste = Mathf.FloorToInt(excessHunger / 5.0f);
            AddWasteProduct(additionalWaste);  // Add 1.0f waste for each 5-point increment
        }

        GD.Print($"Current hunger level: {currentHungerLevel}");

        // Only changes ingredient request if the last attempt to serve it brought it above the threshold for feeding
        if ((currentHungerLevel / maxHungerLevel * 100.0f) > percentageMaxHungerBeforeFeedingRequestMade && !isFeedingRequestSatisfied)
        {
            hasMadeFeedingRequest = false;
            globalSignals.RaiseCreatureFeedRequestSatisfied(true);
            isFeedingRequestSatisfied = true;
        }
    }

    private void HandleAreaCleaned(E_NeedMetAmount amount)
    {
        float newCleanlinessLevel = 0.0f;

        switch (amount)
        {
            case E_NeedMetAmount.NONE:
                // Add anger mutiplied by 1.0
                break;
            case E_NeedMetAmount.HALF:
                newCleanlinessLevel = currentCleanlinessLevel + maxCleanlinessReplenishment * 0.5f;
                currentCleanlinessLevel = Mathf.Min(newCleanlinessLevel, maxCleanlinessLevel);
                // Add anger mutiplied by 0.5
                break;
            case E_NeedMetAmount.MOST:
                newCleanlinessLevel = currentCleanlinessLevel + maxCleanlinessReplenishment * 0.75f;
                currentCleanlinessLevel = Mathf.Min(newCleanlinessLevel, maxCleanlinessLevel);
                // Add anger mutiplied by 0.25
                break;
            case E_NeedMetAmount.ALL:
                newCleanlinessLevel = currentCleanlinessLevel + maxCleanlinessReplenishment * 1.0f;
                currentCleanlinessLevel = Mathf.Min(newCleanlinessLevel, maxCleanlinessLevel);
                break;
            default:
                GD.PrintErr("No valid cleaned area received by CreatureNeeds from CleaningComponent");
                break;
        }

        GD.Print($"Current cleanliness level: {currentCleanlinessLevel}");

        // Only changes cleaning request if the last attempt to clean it brought it above the threshold for cleaning
        if ((currentCleanlinessLevel / maxCleanlinessLevel * 100.0f) > percentageMaxCleanlinessBeforeCleaningRequestMade && !isCleaningRequestSatisfied)
        {
            hasMadeCleaningRequest = false;
            globalSignals.RaiseCreatureCleanRequestSatisfied(true);
            isCleaningRequestSatisfied = true;
        }
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
