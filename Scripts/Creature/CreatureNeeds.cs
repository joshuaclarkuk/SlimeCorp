using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class CreatureNeeds : Node3D
{
    [ExportCategory("Required Nodes")]
    [Export] private FeedingComponent feedingComponentNode = null;
    [Export] private CreatureNeedsDisplay creatureNeedsDisplayNode = null;
    [Export] private Timer needsDisplayUpdateTimerNode = null;

    [ExportCategory("Behaviour")]
    [Export] private float hungerDepletionRate = 0.6f;
    [Export] private float happinessDepletionRate = 0.4f;
    [Export] private float cleanlinessDepletionRate = 0.5f;
    [Export] private float percentageMaxHungerBeforeFeedingRequestMade = 40.0f;
    [Export] private int maxIngredientsRequested = 4;

    private GlobalSignals globalSignals = null;

    private int numIngredientsRequested = 0;

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

    // Feeding requests
    private Array ingredientEnumValues;
    private Dictionary<E_IngredientList, bool> requestedIngredientDictionary = new Dictionary<E_IngredientList, bool>();

    public override void _Ready()
    {
        // Initialise possible ingredients array and compile dictionary
        ingredientEnumValues = Enum.GetValues(typeof(E_IngredientList));

        // Initialise requestedIngredientDictionary with all possible keys and set values to false
        foreach (E_IngredientList ingredient in Enum.GetValues(typeof(E_IngredientList)))
        {
            requestedIngredientDictionary[ingredient] = false;
        }

        // Link global signals so we know when player has clocked in and out
        globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");
        globalSignals.OnPlayerClockedIn += HandlePlayerClockedIn;
        globalSignals.OnPlayerClockedOut += HandlePlayerClockedOut;
        globalSignals.OnServeCreatureFood += HandleServeCreatureFood;

        // Link timer signal to update UI on timeout
        needsDisplayUpdateTimerNode.Timeout += HandleNeedsDisplayUpdateTimerNodeTimeout;
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
            ReduceNeedLevels(delta);
            MakeFeedingRequestIfBelowThreshold();
        }
    }

    // Called by Main to initialise needs per day
    public void SetUpForNewDay(float minutesInDay)
    {
        SetNumberOfMinutesInDay(minutesInDay);
        ResetAllCreatureNeeds();
        creatureNeedsDisplayNode.UpdateProgressBars(currentHungerLevel, currentHappinessLevel, currentCleanlinessLevel, currentTimeLeft);
    }

    private void HandleServeCreatureFood(Dictionary<E_IngredientList, bool> servedIngredients)
    {
        bool hasBeenServedCorrectIngredients = true; // Flag

        foreach (E_IngredientList ingredient in Enum.GetValues(typeof(E_IngredientList)))
        {
            bool requestedValue = requestedIngredientDictionary.ContainsKey(ingredient) ? requestedIngredientDictionary[ingredient] : false;
            bool servedValue = servedIngredients.ContainsKey(ingredient) ? servedIngredients[ingredient] : false;

            // Compare the requested and served values for the current ingredient
            if (requestedValue != servedValue)
            {
                GD.Print($"Mismatch for ingredient {ingredient}: Requested {requestedValue}, Served {servedValue}");
                hasBeenServedCorrectIngredients = false;
            }
        }

        if (hasBeenServedCorrectIngredients)
        {
            GD.Print("All served ingredients match the requested ingredients.");
        }
        else
        {
            GD.Print("There was a mismatch between served and requested ingredients.");
        }
    }

    private void ResetAllCreatureNeeds()
    {
        currentHungerLevel = maxHungerLevel;
        currentHappinessLevel = maxHappinessLevel;
        currentCleanlinessLevel = maxCleanlinessLevel;
    }

    private void SetNumberOfMinutesInDay(float minutes)
    {
        currentTimeLeft = minutes * 60.0f;
    }

    private void ReduceNeedLevels(double delta)
    {
        currentHungerLevel -= (float)delta * hungerDepletionRate;
        currentHappinessLevel -= (float)delta * happinessDepletionRate;
        currentCleanlinessLevel -= (float)delta * cleanlinessDepletionRate;
        currentTimeLeft -= (float)delta;
    }

    private void MakeFeedingRequestIfBelowThreshold()
    {
        if (currentHungerLevel <= maxHungerLevel * (percentageMaxHungerBeforeFeedingRequestMade / 100.0f) && !hasMadeFeedingRequest)
        {
            do
            {
                ProcessFeedingRequest();
            }
            while (requestedIngredientDictionary.Count < 1);  // Ensure we get at least 1 ingredient

            hasMadeFeedingRequest = true;
            globalSignals.RaiseCreatureFeedRequest(requestedIngredientDictionary);

            // Debug request
            foreach (E_IngredientList ingredient in requestedIngredientDictionary.Keys)
            {
                GD.Print($"Creature has requested: {ingredient} and set its value to {requestedIngredientDictionary[ingredient]}");
            }
        }
    }

    private void ProcessFeedingRequest()
    {
        GD.Print("Hunger below threshold. Making feeding request");
        // Make sure all values are set to false before making a new request for safety
        foreach (E_IngredientList ingredient in requestedIngredientDictionary.Keys.ToList())
        {
            requestedIngredientDictionary[ingredient] = false;
        }

        numIngredientsRequested = 0;

        // Loop through possible ingredients and decide whether or not it is wanted
        foreach (E_IngredientList ingredient in ingredientEnumValues)
        {
            // Break early if the max number of ingredients have already been requested
            if (numIngredientsRequested >= maxIngredientsRequested)
            {
                break;
            }

            // Randomly decide if the ingredient is needed (50% chance)
            bool wantsIngredient = GD.Randi() % 2 == 0;

            // If the ingredient is wanted, set it to true in the dictionary
            if (wantsIngredient)
            {
                requestedIngredientDictionary[ingredient] = true;
                numIngredientsRequested++;
            }
        }

        // If no ingredients were selected, warn that new request will be made
        if (numIngredientsRequested == 0)
        {
            GD.Print("No ingredients requested, will retry.");
        }
        else
        {
            // Print requested ingredients
            foreach (var entry in requestedIngredientDictionary)
            {
                GD.Print($"{entry.Key}: Requested = {entry.Value}");
            }
        }
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
