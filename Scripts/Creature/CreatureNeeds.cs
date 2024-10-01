using Godot;
using System;
using System.Collections.Generic;

public partial class CreatureNeeds : Node3D
{
    [ExportCategory("Required Nodes")]
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

    // Feeding request
    private E_IngredientList[] possibleIngredientsArray = null;
    private Dictionary<E_IngredientList, bool> requestedIngredientList = new Dictionary<E_IngredientList, bool>();

    public override void _Ready()
    {
        // Initialise possible ingredients array
        Array enumValues = Enum.GetValues(typeof(E_IngredientList));
        possibleIngredientsArray = new E_IngredientList[enumValues.Length];

        // Assign possibleIngredientsArray values to each of the values in the ingredients enum
        for (int i = 0; i < enumValues.Length; i++)
        {
            possibleIngredientsArray[i] = (E_IngredientList)enumValues.GetValue(i);
        }

        // Link global signals so we know when player has clocked in and out
        globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");
        globalSignals.OnPlayerClockedIn += HandlePlayerClockedIn;
        globalSignals.OnPlayerClockedOut += HandlePlayerClockedOut;

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

    public void BeServedFood(List<E_IngredientList> ingredientsBeingServed)
    {
        // Check for unwanted ingredients        
        bool hasExtraIngredients = false;

        // Mark requested ingredients as true if they are served
        foreach (E_IngredientList servedIngredient in ingredientsBeingServed)
        {
            if (requestedIngredientList.ContainsKey(servedIngredient))
            {
                requestedIngredientList[servedIngredient] = true;
            }
            else
            {
                hasExtraIngredients = true;
            }
        }

        // Check for missing ingredients
        List<E_IngredientList> missingIngredients = new List<E_IngredientList>();
        foreach (KeyValuePair<E_IngredientList, bool> ingredient in requestedIngredientList)
        {
            if (!ingredient.Value)
            {
                missingIngredients.Add(ingredient.Key);  // Add missing ingredients to list
            }
        }


        // Print debug information
        GD.Print("=== Requested Ingredients Status ===");
        foreach (KeyValuePair<E_IngredientList, bool> entry in requestedIngredientList)
        {
            GD.Print($"Ingredient: {entry.Key}, Served: {entry.Value}");
        }
        GD.Print("====================================");

        // Check for errors
        if (missingIngredients.Count > 0)
        {
            GD.PrintErr("Missing Ingredients: " + string.Join(", ", missingIngredients));
        }

        if (hasExtraIngredients)
        {
            GD.PrintErr("There are extra ingredients that shouldn't have been served.");
        }

        if (missingIngredients.Count == 0 && !hasExtraIngredients)
        {
            GD.Print("All ingredients are correct!");
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
            while (requestedIngredientList.Count < 1);  // Ensure we get at least 1 ingredient

            hasMadeFeedingRequest = true;
            globalSignals.RaiseCreatureFeedRequest(requestedIngredientList);

            // Debug request
            foreach (E_IngredientList ingredient in requestedIngredientList.Keys)
            {
                GD.Print($"Creature has requested: {ingredient} and set its value to {requestedIngredientList[ingredient]}");
            }
        }
    }

    private void ProcessFeedingRequest()
    {
        GD.Print("Hunger below threshold. Making feeding request");
        // Clear list and reset counters for a new request
        requestedIngredientList.Clear();
        numIngredientsRequested = 0;

        // Loop through possible ingredients and decide whether or not it is wanted
        foreach (E_IngredientList ingredient in possibleIngredientsArray)
        {
            // Break early if max ingredients have already been requested
            if (numIngredientsRequested >= maxIngredientsRequested)
            {
                break;
            }

            // Randomly decide if the ingredient is needed, with a 50% chance
            bool wantsIngredient = GD.Randi() % 2 == 0;

            if (wantsIngredient)
            {
                requestedIngredientList.Add(ingredient, false);
                numIngredientsRequested++;
            }
        }

        // If the list is empty, warn that a new request will be made
        if (requestedIngredientList.Count == 0)
        {
            GD.Print("No ingredients requested, will retry.");
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
