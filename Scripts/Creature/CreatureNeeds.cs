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

    [ExportCategory("Max Levels")]
    [Export] private float maxHungerLevel = 100.0f;
    [Export] private float maxHappinessLevel = 100.0f;
    [Export] private float maxCleanlinessLevel = 100.0f;

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

    [ExportCategory("Slime Collection")]
    [Export] private float baseSlimeCollectionRate = 1.0f;
    [Export] private float maxSlimeInCanister = 100.0f;
    [Export] private float feedingSlimeMultiplier = 2.0f;
    [Export] private float cleaningSlimeMultiplier = 2.0f;
    [Export] private float happinessSlimeMultiplier = 2.0f;

    private GlobalSignals globalSignals = null;

    // Current levels
    private float currentHungerLevel = 0.0f;
    private float currentHappinessLevel = 0.0f;
    private float currentCleanlinessLevel = 0.0f;
    private float currentTimeLeft = 0.0f;
    private float currentSlimeLevel = 0.0f;

    // Guards
    private bool playerHasClockedIn = false;
    private bool hasMadeFeedingRequest = false;
    private bool hasMadeCleaningRequest = false;
    private bool isFeedingRequestSatisfied = false;
    private bool isCleaningRequestSatisfied = false;

    public override void _Ready()
    {
        globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");
        SubscribeToEvents();
    }

    public override void _ExitTree()
    {
        UnsubscribeFromEvents();
    }

    public override void _Process(double delta)
    {
        if (playerHasClockedIn)
        {
            ReduceNeedLevels(delta);
            AddSlimeToCanister(delta);
        }
    }

    // Called by Main to initialise needs per day
    public void SetUpForNewDay(float minutesInDay)
    {
        SetNumberOfMinutesInDay(minutesInDay);
        ResetAllCreatureNeeds();
        creatureNeedsDisplayNode.UpdateProgressBars(currentHungerLevel, currentHappinessLevel, currentCleanlinessLevel, currentTimeLeft);
        debugUINode.UpdateProgressBars(currentHungerLevel, currentHappinessLevel, currentCleanlinessLevel, currentTimeLeft, currentSlimeLevel);
    }

    private void SubscribeToEvents()
    {
        // Link global signals so we know when player has clocked in and out
        globalSignals.OnPlayerClockedIn += HandlePlayerClockedIn;
        globalSignals.OnPlayerClockedOut += HandlePlayerClockedOut;

        // Link timer signal to update UI on timeout
        needsDisplayUpdateTimerNode.Timeout += HandleNeedsDisplayUpdateTimerNodeTimeout;

        // Link NeedComponent signals
        feedingComponentNode.OnCreatureServedFood += HandleCreatureServedFood;
        cleaningComponentNode.OnAreaCleaned += HandleAreaCleaned;

        // Link slime barrel attached events

    }

    private void UnsubscribeFromEvents()
    {
        needsDisplayUpdateTimerNode.Timeout -= HandleNeedsDisplayUpdateTimerNodeTimeout;
        globalSignals.OnPlayerClockedIn -= HandlePlayerClockedIn;
        globalSignals.OnPlayerClockedOut -= HandlePlayerClockedOut;

        // Unlink NeedComponent signals
        feedingComponentNode.OnCreatureServedFood -= HandleCreatureServedFood;
        cleaningComponentNode.OnAreaCleaned -= HandleAreaCleaned;
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
        currentHungerLevel = Mathf.Max(0, currentHungerLevel - hungerDepletionRate * (float) delta);
        currentHappinessLevel = Mathf.Max(0, currentHappinessLevel - happinessDepletionRate * (float)delta);
        currentCleanlinessLevel = Mathf.Max(0, currentCleanlinessLevel - cleanlinessDepletionRate * (float)delta);
        currentTimeLeft = Mathf.Max(0, currentTimeLeft - (float)delta);

        MakeFeedingRequestIfBelowThreshold();
        MakeCleaningRequestIfBelowThreshold();
    }

    private void AddSlimeToCanister(double delta)
    {
        // Slime to add from food, cleanliness, and happiness
        float slimeToAddFromFood = baseSlimeCollectionRate;
        float slimeToAddFromCleanliness = baseSlimeCollectionRate;
        float slimeToAddFromHappiness = baseSlimeCollectionRate;

        float hungerPercentage = (currentHungerLevel / maxHungerLevel) * 100.0f;
        switch (hungerPercentage)
        {
            case > 100.0f:
                slimeToAddFromFood *= feedingSlimeMultiplier * 1.2f;  // Above max hunger, add bonus
                break;
            case >= 80.0f:
                slimeToAddFromFood *= feedingSlimeMultiplier;  // Above 80%, multiply
                break;
            case < 20.0f:
                slimeToAddFromFood *= 0.5f; // Below 20% incur penalty
                    break;
            default:
                slimeToAddFromFood *= 1.0f;  // Normal collection rate
                break;
        }

        float cleanlinessPercentage = (currentCleanlinessLevel / maxCleanlinessLevel) * 100.0f;
        switch (cleanlinessPercentage)
        {
            case > 100.0f:
                slimeToAddFromCleanliness *= cleaningSlimeMultiplier * 1.2f;  // Above max hunger, add bonus
                break;
            case >= 80.0f:
                slimeToAddFromCleanliness *= cleaningSlimeMultiplier;  // Above 80%, multiply
                break;
            case < 20.0f:
                slimeToAddFromCleanliness *= 0.5f; // Below 20% incur penalty
                break;
            default:
                slimeToAddFromCleanliness *= 1.0f;  // Normal collection rate
                break;
        }

        float happinessPercentage = (currentHappinessLevel / maxHappinessLevel) * 100.0f;
        switch (happinessPercentage)
        {
            case > 100.0f:
                slimeToAddFromHappiness *= happinessSlimeMultiplier * 1.2f;  // Above max hunger, add bonus
                break;
            case >= 80.0f:
                slimeToAddFromHappiness *= happinessSlimeMultiplier;  // Above 80%, multiply
                break;
            case < 20.0f:
                slimeToAddFromHappiness *= 0.5f; // Below 20% incur penalty
                break;
            default:
                slimeToAddFromHappiness *= 1.0f;  // Normal collection rate
                break;
        }

        float totalSlimeToAdd = (slimeToAddFromFood + slimeToAddFromCleanliness + slimeToAddFromHappiness) * (float)delta;
        currentSlimeLevel = Mathf.Min(currentSlimeLevel + totalSlimeToAdd, maxSlimeInCanister); // Ensure it doesn't exceed max capacity

        GD.Print($"Slime added: {totalSlimeToAdd}, Current slime level: {currentSlimeLevel}");
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
        debugUINode.UpdateProgressBars(currentHungerLevel, currentHappinessLevel, currentCleanlinessLevel, currentTimeLeft, currentSlimeLevel); // DEBUG TO REMOVE
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
