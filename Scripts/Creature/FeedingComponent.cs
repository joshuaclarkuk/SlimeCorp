using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class FeedingComponent : Node3D
{
    [ExportCategory("Behaviour")]
    [Export] private int maxIngredientsRequested = 4;

    private GlobalSignals globalSignals = null;

    // Feeding requests
    private Array ingredientEnumValues;
    private Dictionary<E_IngredientList, bool> requestedIngredientDictionary = new Dictionary<E_IngredientList, bool>();
    private int numIngredientsRequested = 0;

    public event Action<E_NeedMetAmount> OnCreatureServedFood;

    public override void _Ready()
    {
        // Link signals
        globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");
        globalSignals.OnServeCreatureFood += HandleServeCreatureFood;

        // Initialise possible ingredients array and compile dictionary
        ingredientEnumValues = Enum.GetValues(typeof(E_IngredientList));

        // Initialise requestedIngredientDictionary with all possible keys and set values to false
        foreach (E_IngredientList ingredient in Enum.GetValues(typeof(E_IngredientList)))
        {
            requestedIngredientDictionary[ingredient] = false;
        }
    }

    public override void _ExitTree()
    {
        globalSignals.OnServeCreatureFood -= HandleServeCreatureFood;
    }

    public void ProcessFeedingRequest()
    {
        // Make sure all values are set to false before making a new request for safety
        foreach (E_IngredientList ingredient in requestedIngredientDictionary.Keys.ToList())
        {
            requestedIngredientDictionary[ingredient] = false;
        }

        numIngredientsRequested = 0;

        // Hoping this makes ingredient selection more uniform rather than just running top to bottom
        List<E_IngredientList> shuffledIngredients = ingredientEnumValues.Cast<E_IngredientList>().OrderBy(x => GD.Randf()).ToList();

        // Loop through possible ingredients and decide whether or not it is wanted
        foreach (E_IngredientList ingredient in shuffledIngredients)
        {
            // Break early if the max number of ingredients have already been requested
            if (numIngredientsRequested >= maxIngredientsRequested)
            {
                break;
            }

            // Randomly decide if the ingredient is needed (50% chance)
            bool wantsIngredient = GD.Randf() < 0.5f;

            // If the ingredient is wanted, set it to true in the dictionary
            if (wantsIngredient)
            {
                requestedIngredientDictionary[ingredient] = true;
                numIngredientsRequested++;
            }
        }

        // If no ingredients were selected, make another request
        if (numIngredientsRequested == 0)
        {
            GD.Print("No ingredients requested, will retry.");
            ProcessFeedingRequest();
        }
        else
        {
            globalSignals.RaiseCreatureFeedRequest(requestedIngredientDictionary);

            // Debug request
            foreach (E_IngredientList ingredient in requestedIngredientDictionary.Keys)
            {
                GD.Print($"Creature has requested: {ingredient} and set its value to {requestedIngredientDictionary[ingredient]}");
            }
        }
    }

    private void HandleServeCreatureFood(Dictionary<E_IngredientList, bool> servedIngredients)
    {
        int numberOfRequestedIngredients = requestedIngredientDictionary.Values.Count(value => value == true);
        int numberOfCorrectIngredientsServed = 0;

        foreach (E_IngredientList ingredient in Enum.GetValues(typeof(E_IngredientList)))
        {
            bool requestedValue = requestedIngredientDictionary.ContainsKey(ingredient) ? requestedIngredientDictionary[ingredient] : false;
            bool servedValue = servedIngredients.ContainsKey(ingredient) ? servedIngredients[ingredient] : false;

            // Compare the requested and served values for the current ingredient
            if (requestedValue && servedValue)
            {
                numberOfCorrectIngredientsServed++;
            }
        }

        // Calculate percentage of correct ingredients based on number requested
        float percentageCorrectIngredients = (float)numberOfCorrectIngredientsServed / numberOfRequestedIngredients * 100;
        GD.Print($"Percentage correct: {percentageCorrectIngredients}");

        // Switch statement based on percentage of correct ingredients
        switch (percentageCorrectIngredients)
        {
            case <= 50.0f:
                GD.Print("Less than or equal to 50% of ingredients served correctly. Apply penalty.");
                OnCreatureServedFood?.Invoke(E_NeedMetAmount.NONE);
                break;

            case > 50.0f and <= 75.0f:
                GD.Print("Between 51% and 75% of ingredients served correctly. Neutral outcome.");
                OnCreatureServedFood?.Invoke(E_NeedMetAmount.HALF);
                break;

            case > 75.0f and < 100.0f:
                GD.Print("Between 76% and 99% of ingredients served correctly. Good outcome.");
                OnCreatureServedFood?.Invoke(E_NeedMetAmount.MOST);
                break;

            case 100.0f:
                GD.Print("100% of ingredients served correctly. Perfect score.");
                OnCreatureServedFood?.Invoke(E_NeedMetAmount.ALL);
                break;
        }
    }
}
