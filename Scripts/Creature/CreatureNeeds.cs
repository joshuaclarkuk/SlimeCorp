using Godot;
using System;
using System.Collections.Generic;

public partial class CreatureNeeds : Node3D
{
    [ExportCategory("Required Nodes")]
    [Export] private Timer feedingRequestTimerNode = null;
    [Export] private Timer cleaningRequestTimerNode = null;
    [Export] private Timer needsDisplayUpdateTimerNode = null;
    [Export] private Timer failureStateTimerNode = null;

    [ExportCategory("External Nodes")]
    [Export] private CreatureNeedsDisplay creatureNeedsDisplayNode = null;
    [Export] private SlimeCollectionStation slimeCollectionStationNode = null;
    [Export] private DebugUI debugUINode = null;

    [ExportCategory("Max Levels")]
    [Export] private float maxHungerLevel = 100.0f;
    [Export] private float maxHappinessLevel = 100.0f;
    [Export] private float maxCleanlinessLevel = 100.0f;
    [Export] private float maxAngerLevel = 100.0f;

    [ExportCategory("Depletion Rates")]
    [Export] private float hungerDepletionRate = 0.2f;
    [Export] private float happinessDepletionRate = 0.15f;
    [Export] private float cleanlinessDepletionRate = 0.1f;
    [Export] private float angerDepletionRate = 0.1f;

    [ExportCategory("Request Points (Percentage Left)")]
    [Export] private float percentageMaxHungerBeforeFeedingRequestMade = 40.0f;
    [Export] private float percentageMaxCleanlinessBeforeCleaningRequestMade = 30.0f;
    [Export] private float percentageMaxHappinessBeforePlayRequestMade = 20.0f;

    [ExportCategory("Replenishment/Addition Rates")]
    [Export] private float maxHungerReplenishment = 20.0f;
    [Export] private float maxCleanlinessReplenishment = 20.0f;
    [Export] private float maxHappinessReplenishment = 5.0f;
    [Export] private float maxWasteProductToAddAfterFeeding = 5.0f;
    [Export] private float maxAngerPointsToAdd = 10.0f;

    private GlobalSignals globalSignals = null;

    // Current levels
    private float currentHungerLevel = 0.0f;
    private float currentHappinessLevel = 0.0f;
    private float currentCleanlinessLevel = 0.0f;
    private float currentAngerLevel = 0.0f;
    private float currentTimeLeft = 0.0f;

    // Max possible requests
    private int maxFeedingRequests = 0;
    private int maxCleaningRequests = 0;
    private int activeFeedingRequests = 0;
    private int activeCleaningRequests = 0;

    // Request lists
    private Array ingredientEnumValues;
    private Array areaEnumValues;
    private List<E_IngredientList> requestedIngredientList = new List<E_IngredientList>();
    private List<E_AreasToClean> requestedAreasList = new List<E_AreasToClean>();

    // Guards
    private bool playerHasClockedIn = false;
    private bool isFailureStateTimerStarted = false;

    public override void _Ready()
    {
        globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");

        SubscribeToEvents();

        // Initialise enum arrays and clear "requested" lists
        ingredientEnumValues = Enum.GetValues(typeof(E_IngredientList));
        areaEnumValues = Enum.GetValues(typeof(E_AreasToClean));
        requestedIngredientList.Clear();
        requestedAreasList.Clear();
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
            slimeCollectionStationNode.AddSlimeToCanister(delta, currentHungerLevel, maxHungerLevel, currentCleanlinessLevel, maxCleanlinessLevel, currentHappinessLevel, maxHappinessLevel);
        }
    }

    private void SubscribeToEvents()
    {
        // Link global signals so we know when player has clocked in and out
        globalSignals.OnPlayerClockedIn += HandlePlayerClockedIn;
        globalSignals.OnPlayerClockedOut += HandlePlayerClockedOut;

        // Link timer signal to update UI on timeout
        needsDisplayUpdateTimerNode.Timeout += HandleNeedsDisplayUpdateTimerNodeTimeout;

        // Link NeedComponent signals
        globalSignals.OnServeCreatureFood += HandleServeCreatureFood;
        globalSignals.OnAreaCleaned += HandleAreaCleaned;
        globalSignals.OnCreaturePlayedWith += HandleCreaturePlayedWith;

        // Link timeout signals
        feedingRequestTimerNode.Timeout += HandleFeedingRequestTimerTimeout;
        cleaningRequestTimerNode.Timeout += HandleCleaningRequestTimerTimeout;
        failureStateTimerNode.Timeout += HandleFailureStateTimerTimeout;
    }

    private void UnsubscribeFromEvents()
    {
        needsDisplayUpdateTimerNode.Timeout -= HandleNeedsDisplayUpdateTimerNodeTimeout;
        globalSignals.OnPlayerClockedIn -= HandlePlayerClockedIn;
        globalSignals.OnPlayerClockedOut -= HandlePlayerClockedOut;
        globalSignals.OnServeCreatureFood -= HandleServeCreatureFood;
        globalSignals.OnAreaCleaned -= HandleAreaCleaned;
        globalSignals.OnCreaturePlayedWith -= HandleCreaturePlayedWith;
        feedingRequestTimerNode.Timeout -= HandleFeedingRequestTimerTimeout;
        cleaningRequestTimerNode.Timeout -= HandleCleaningRequestTimerTimeout;
        failureStateTimerNode.Timeout -= HandleFailureStateTimerTimeout;
    }

    public void SetCreatureNeedDepletionRatesFromResource(float hungerDepletionRate, float happinessDepletionRate, float cleanlinessDepletionRate)
    {
        this.hungerDepletionRate = hungerDepletionRate;
        this.happinessDepletionRate = happinessDepletionRate;
        this.cleanlinessDepletionRate = cleanlinessDepletionRate;
    }

    public void SetMaxNeedReplenishmentRatesFromResource(float maxHungerReplenishment, float maxCleanlinessReplenishment, float maxHappinessReplenishment, float maxWasteProductToAddAfterFeeding)
    {
        this.maxHungerReplenishment = maxHungerReplenishment;
        this.maxCleanlinessReplenishment = maxCleanlinessReplenishment;
        this.maxHappinessReplenishment = maxHappinessReplenishment;
        this.maxWasteProductToAddAfterFeeding = maxWasteProductToAddAfterFeeding;
    }

    public void SetRequestTimers()
    {
        // Calculation of depletion over replenishment ensures that requests are made in line with how much the value is replenished
        float hungerDepletionTime = hungerDepletionRate > 0 ? (maxHungerReplenishment / hungerDepletionRate) * 0.75f : float.PositiveInfinity; // Ensures timer doesn't go off on first level if station isn't active
        feedingRequestTimerNode.WaitTime = hungerDepletionTime;

        float cleanlinessDepletionTime = cleanlinessDepletionRate > 0 ? (maxCleanlinessReplenishment / cleanlinessDepletionRate) * 0.75f : float.PositiveInfinity; // Ensures timer doesn't go off on first level if station isn't active
        cleaningRequestTimerNode.WaitTime = cleanlinessDepletionTime;

        // Print debug information
        GD.Print($"FeedTimer wait time is: {feedingRequestTimerNode.WaitTime}, cleanTimer wait time is: {cleaningRequestTimerNode.WaitTime}");
    }


    public void SetMaxRequests()
    {
        maxFeedingRequests = Mathf.RoundToInt(maxHungerLevel / maxHungerReplenishment) + 1;
        maxCleaningRequests = Mathf.RoundToInt(maxCleanlinessLevel / maxCleanlinessReplenishment) + 1;
        GD.Print($"Max feeding requests: {maxFeedingRequests}, max cleaning requests: {maxCleaningRequests}");
    }

    // Called by Main to initialise needs per day
    public void SetUpForNewDay(int minutesInDay)
    {
        SetNumberOfMinutesInDay(minutesInDay);
        ResetAllCreatureNeeds();
        requestedIngredientList.Clear();
        requestedAreasList.Clear();
        creatureNeedsDisplayNode.UpdateProgressBars(currentHungerLevel, currentHappinessLevel, currentCleanlinessLevel, currentTimeLeft);
        debugUINode.UpdateProgressBars(currentHungerLevel, currentHappinessLevel, currentCleanlinessLevel, currentTimeLeft);
    }

    private void SetNumberOfMinutesInDay(int minutes)
    {
        currentTimeLeft = (float)minutes * 60.0f;
    }

    private void ResetAllCreatureNeeds()
    {
        currentHungerLevel = maxHungerLevel;
        currentHappinessLevel = maxHappinessLevel;
        currentCleanlinessLevel = maxCleanlinessLevel;
        currentAngerLevel = 0.0f;
    }

    private void ReduceNeedLevels(double delta)
    {
        currentHungerLevel = Mathf.Max(0, currentHungerLevel - hungerDepletionRate * (float)delta);
        currentHappinessLevel = Mathf.Max(0, currentHappinessLevel - happinessDepletionRate * (float)delta);
        currentCleanlinessLevel = Mathf.Max(0, currentCleanlinessLevel - cleanlinessDepletionRate * (float)delta);
        angerDepletionRate = Mathf.Max(0, currentAngerLevel - angerDepletionRate * (float)delta);
        currentTimeLeft = Mathf.Max(0, currentTimeLeft - (float)delta);

        CheckIfFailingNeedsAndStartFailureTimer();
        CheckIfShiftIsOver();
    }

    private void HandleNeedsDisplayUpdateTimerNodeTimeout()
    {
        float newHungerPercentage = currentHungerLevel / maxHungerLevel * 100.0f;
        float newHappinessPercentage = currentHappinessLevel / maxHappinessLevel * 100.0f;
        float newCleanlinessPercentage = currentCleanlinessLevel / maxCleanlinessLevel * 100.0f;
        creatureNeedsDisplayNode.UpdateProgressBars(newHungerPercentage, newHappinessPercentage, newCleanlinessPercentage, currentTimeLeft);
        debugUINode.UpdateProgressBars(currentHungerLevel, currentHappinessLevel, currentCleanlinessLevel, currentTimeLeft); // DEBUG TO REMOVE
    }

    private void HandleFeedingRequestTimerTimeout()
    {
        GD.Print("Hunger timer timed out. Making feeding request");
        if (activeFeedingRequests < maxFeedingRequests)
        {
            ProcessFeedingRequest();
        }
    }

    private void HandleCleaningRequestTimerTimeout()
    {
        GD.Print("Cleaning timer timed out. Making cleaning request");
        if (activeCleaningRequests < maxCleaningRequests)
        {
            ProcessCleaningRequest();
        }
    }

    private void ProcessFeedingRequest()
    {
        GD.Print("Processing feeding request");
        int randomIngredientIndex = GD.RandRange(0, ingredientEnumValues.Length - 1);
        E_IngredientList randomIngredient = (E_IngredientList)ingredientEnumValues.GetValue(randomIngredientIndex);
        requestedIngredientList.Add(randomIngredient);
        globalSignals.RaiseCreatureFeedRequest(requestedIngredientList);
        activeFeedingRequests++;
        
        // Debug
        debugUINode.UpdateFoodRequestList(requestedIngredientList);
    }

    private void ProcessCleaningRequest()
    {
        GD.Print("Processing cleaning request");
        int randomAreaIndex = GD.RandRange(0, areaEnumValues.Length - 1);
        E_AreasToClean randomArea = (E_AreasToClean)areaEnumValues.GetValue(randomAreaIndex);
        requestedAreasList.Add(randomArea);
        globalSignals.RaiseAreaToCleanRequest(requestedAreasList);
        activeCleaningRequests++;

        // Debug
        debugUINode.UpdateCleaningRequestList(requestedAreasList);
    }

    private void HandleServeCreatureFood(List<E_IngredientList> servedIngredients)
    {
        foreach (E_IngredientList ingredient in servedIngredients)
        {
            if (requestedIngredientList.Contains(ingredient))
            {
                // Good outcome
                currentHungerLevel += maxHungerReplenishment;
                requestedIngredientList.Remove(ingredient);
                AddWasteProduct(maxWasteProductToAddAfterFeeding);
                activeFeedingRequests--;
            }
            else
            {
                // Bad outcome
                currentAngerLevel += maxAngerPointsToAdd;
            }
        }
        debugUINode.UpdateFoodRequestList(requestedIngredientList);
    }

    private void HandleAreaCleaned(List<E_AreasToClean> areaCleaned)
    {
        foreach (E_AreasToClean area in areaCleaned)
        {
            if (requestedAreasList.Contains(area))
            {
                currentCleanlinessLevel += maxCleanlinessReplenishment;
                requestedAreasList.Remove(area);
                activeCleaningRequests--;
            }
            else
            {
                currentAngerLevel += maxAngerPointsToAdd;
            }
        }
        debugUINode.UpdateCleaningRequestList(areaCleaned);
    }

    private void AddWasteProduct(float amountOfWaste)
    {
        currentCleanlinessLevel -= amountOfWaste;
    }

    private void HandleCreaturePlayedWith()
    {
        float newHappinessLevel = 0.0f;

        newHappinessLevel = currentHappinessLevel + maxHappinessReplenishment;
        currentHappinessLevel = Mathf.Min(newHappinessLevel, maxHappinessLevel);

        GD.Print($"Current happiness level: {currentHappinessLevel}");
    }

    private void CheckIfFailingNeedsAndStartFailureTimer()
    {
        if (currentHungerLevel > 0.0f && currentHappinessLevel > 0.0f && currentCleanlinessLevel > 0.0f)
        {
            if (isFailureStateTimerStarted)
            {
                failureStateTimerNode.Stop();
                isFailureStateTimerStarted = false;
                globalSignals.RaisePlayerAtRiskOfFailing(false);
            }
        }
        else
        {
            if (!isFailureStateTimerStarted)
            {
                BeginFailureCountdown();
                globalSignals.RaisePlayerAtRiskOfFailing(true);
            }
        }
    }

    private void CheckIfShiftIsOver()
    {
        if (currentTimeLeft <= 0.0f)
        {
            globalSignals.RaiseShiftIsOver();
        }
    }

    private void BeginFailureCountdown()
    {
        failureStateTimerNode.Start();
        isFailureStateTimerStarted = true;

        // Build up some tense music
        // Change creature's eye to red 

        GD.PrintErr("Failure timer started");
    }

    private void HandleFailureStateTimerTimeout()
    {
        GD.PrintErr("Player failed");
        globalSignals.RaisePlayerFailureState();
    }

    private void HandlePlayerClockedIn()
    {
        playerHasClockedIn = true;
        needsDisplayUpdateTimerNode.Start();
        feedingRequestTimerNode.Start();
        cleaningRequestTimerNode.Start();
    }

    private void HandlePlayerClockedOut()
    {
        playerHasClockedIn = false;
        needsDisplayUpdateTimerNode.Stop();
        feedingRequestTimerNode.Stop();
        cleaningRequestTimerNode.Stop();
    }
}
