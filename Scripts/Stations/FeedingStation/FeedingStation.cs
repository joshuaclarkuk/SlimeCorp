using Godot;
using System;
using System.Collections.Generic;

public partial class FeedingStation : Station
{
    [ExportCategory("Required Nodes")]
    [Export] private Lever leverNode = null;
    [Export] private Timer servingFoodTimerNode = null;

    private List<E_IngredientList> activeIngredients = new List<E_IngredientList>();

    public override void _Ready()
    {
        base._Ready();

        // Store ingredient enum values
        Array enumValues = Enum.GetValues(typeof(E_IngredientList));

        // Initialise ingredient dictionary with all possible ingredients and set initial values to false
        InitialiseIngredientDictionary(enumValues);

        // Assign button debug label values (leaves 0 clear for "Reset")
        AssignDebugLabelValues(enumValues);

        // Have to link in _Ready and _Exit otherwise would lose track of timer when player exits Station
        servingFoodTimerNode.Timeout += HandleServingFoodTimerTimeout;
    }

    public override void _ExitTree()
    {
        servingFoodTimerNode.Timeout -= HandleServingFoodTimerTimeout;
    }

    public override void EnterStation()
    {
        base.EnterStation();

        // Link signals
        leverNode.OnLeverTargetReached += HandleLeverTargetReached;

        GD.Print($"Calling EnterStation method on {Name}");
    }

    public override void ExitStation()
    {
        base.ExitStation();

        // Unlink signals
        leverNode.OnLeverTargetReached -= HandleLeverTargetReached;

        // Reset machine
        ResetMachine();

        GD.Print($"Calling ExitStation method on {Name}");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

        if (isMouseClicked)
        {
            if (canInteractWithStation)
            {
                leverNode.MovePhysicalHandleWithMouseMotion(mouseDragSensitivity, mouseDragMotion);
            }
        }
        else
        {
            leverNode.ReturnToOriginalPosition();
        }
    }

    public void ResetMachine()
    {
        buttonsNode.ResetAndRaiseAllButtons();
        leverNode.ReturnToOriginalPosition();
    }

    protected override void HandleButtonEngaged(int buttonIndex)
    {
        switch (buttonIndex)
        {
            case 0:
                // Execute behavior for button 0
                buttonsNode.ResetAndRaiseAllButtons();
                break;
            case 1:
                // Execute behavior for button 1
                AddIngredientToList(0);
                break;
            case 2:
                // Execute behavior for button 2
                AddIngredientToList(1);
                break;
            case 3:
                // Execute behavior for button 3
                AddIngredientToList(2);
                break;
            case 4:
                // Execute behavior for button 4
                AddIngredientToList(3);
                break;
            case 5:
                // Execute behavior for button 5
                AddIngredientToList(4);
                break;
            case 6:
                // Execute behavior for button 6
                AddIngredientToList(5);
                break;
            case 7:
                // Execute behavior for button 7
                AddIngredientToList(6);
                break;
            case 8:
                // Execute behavior for button 8
                AddIngredientToList(7);
                break;
            case 9:
                // Execute behavior for button 9
                AddIngredientToList(8);
                break;
            default:
                GD.Print("Unhandled button press");
                break;
        }
    }

    protected override void HandleButtonDisengaged(int buttonIndex)
    {
        switch (buttonIndex)
        {
            case 0:
                // Execute behavior for button 0
                break;
            case 1:
                // Execute behavior for button 1
                RemoveIngredientFromList(0);
                break;
            case 2:
                // Execute behavior for button 2
                RemoveIngredientFromList(1);
                break;
            case 3:
                // Execute behavior for button 3
                RemoveIngredientFromList(2);
                break;
            case 4:
                // Execute behavior for button 4
                RemoveIngredientFromList(3);
                break;
            case 5:
                // Execute behavior for button 5
                RemoveIngredientFromList(4);
                break;
            case 6:
                // Execute behavior for button 6
                RemoveIngredientFromList(5);
                break;
            case 7:
                // Execute behavior for button 7
                RemoveIngredientFromList(6);
                break;
            case 8:
                // Execute behavior for button 8
                RemoveIngredientFromList(7);
                break;
            case 9:
                // Execute behavior for button 9
                RemoveIngredientFromList(8);
                break;
            default:
                GD.Print("Unhandled button press");
                break;
        }
    }

    private void InitialiseIngredientDictionary(Array enumValues)
    {
        activeIngredients.Clear();
    }

    private void AssignDebugLabelValues(Array enumValues)
    {
        // Set button 0 to be "Reset"
        buttonsNode.ButtonArray[0].AssignDebugLabelText(" 0\nReset");

        // Assign buttons 1 - 9 to be results of loop through enum
        int enumIndex = 0;
        for (int j = 1; j < buttonsNode.ButtonArray.Length; j++)
        {
            if (enumIndex < enumValues.Length)
            {
                buttonsNode.ButtonArray[j].AssignDebugLabelText($"{j}\n{enumValues.GetValue(enumIndex)}");
                enumIndex++;
            }
        }
    }

    private void AddIngredientToList(int ingredientIndex)
    {
        E_IngredientList ingredientEnum = (E_IngredientList)ingredientIndex;
        activeIngredients.Add(ingredientEnum);

        GD.Print("Active ingredients:");
        foreach (E_IngredientList ingredient in activeIngredients)
        {
            GD.Print(ingredient.ToString());
        }
    }

    private void RemoveIngredientFromList(int ingredientIndex)
    {
        E_IngredientList ingredientEnum = (E_IngredientList)ingredientIndex;
        activeIngredients.Remove(ingredientEnum);
    }

    private void HandleLeverTargetReached()
    {
        servingFoodTimerNode.Start();
        canInteractWithStation = false;
        globalSignals.RaiseServeCreatureFood(activeIngredients);
        globalSignals.RaisePlayerExitStation(StationType);
    }

    private void HandleServingFoodTimerTimeout()
    {
        canInteractWithStation = true;
    }
}