using Godot;
using System;
using System.Collections.Generic;

public partial class FeedingStation : Station
{
    [ExportCategory("Required Nodes")]
    [Export] private Lever leverNode = null;
    [Export] private Timer servingFoodTimerNode = null;

    private Dictionary<E_IngredientList, bool> activeIngredients = new Dictionary<E_IngredientList, bool>();

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
        buttonsNode.ResetAndRaiseAllButtons();
        leverNode.ReturnToOriginalPosition();

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

    protected override void HandleButtonEngaged(int buttonIndex)
    {
        GD.Print($"Button {buttonIndex} was pressed in {Name}.");

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
        GD.Print($"Button {buttonIndex} was released in FeedingStation.");

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

    private void AddIngredientToList(int ingredientIndex)
    {
        E_IngredientList ingredientEnum = (E_IngredientList)ingredientIndex;
        activeIngredients[ingredientEnum] = true;
        GD.Print($"Ingredient added: {activeIngredients[ingredientEnum]}");

        foreach (KeyValuePair<E_IngredientList, bool> entry in activeIngredients)
        {
            GD.Print($"Ingredient: {entry.Key}, Served: {entry.Value}");
        }
    }

    private void RemoveIngredientFromList(int ingredientIndex)
    {
        E_IngredientList ingredientEnum = (E_IngredientList)ingredientIndex;
        activeIngredients[ingredientEnum] = false;
        GD.Print($"Ingredient removed: {activeIngredients[ingredientEnum]}");

        foreach (KeyValuePair<E_IngredientList, bool> entry in activeIngredients)
        {
            GD.Print($"Ingredient: {entry.Key}, Served: {entry.Value}");
        }
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

    private void InitialiseIngredientDictionary(Array enumValues)
    {
        for (int i = 0; i < enumValues.Length; i++)
        {
            activeIngredients.Add((E_IngredientList)enumValues.GetValue(i), false);
        }
    }

    private void AssignDebugLabelValues(Array enumValues)
    {
        buttonsNode.ButtonArray[0].AssignDebugLabelText("Reset");
        int enumIndex = 0;
        for (int j = 1; j < buttonsNode.ButtonArray.Length; j++)
        {
            if (enumIndex < enumValues.Length)
            {
                buttonsNode.ButtonArray[j].AssignDebugLabelText(enumValues.GetValue(enumIndex).ToString());
                enumIndex++;
            }
        }
    }
}