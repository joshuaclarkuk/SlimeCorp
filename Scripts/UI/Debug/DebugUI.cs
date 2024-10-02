using Godot;
using System;
using System.Collections.Generic;

public partial class DebugUI : Control
{
    [ExportCategory("Required Nodes")]
    [Export] private Label employeeNumberLabelNode = null;
    [Export] private VBoxContainer foodRequestContainerNode = null;
    [Export] private VBoxContainer areaCleanRequestContainerNode = null;
    [Export] private ProgressBar hungerProgressBar = null;
    [Export] private ProgressBar happinessProgressBar = null;
    [Export] private ProgressBar cleanlinessProgressBar = null;
    [Export] private Label timeLeftText = null;

    [ExportCategory("Scenes To Instantiate")]
    [Export] private PackedScene foodRequestScene = null;
    [Export] private PackedScene areaCleanRequestScene = null;

    private GlobalSignals globalSignals = null;

    public override void _Ready()
    {
        globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");
        globalSignals.OnEmployeeNumberGenerated += HandleEmployeeNumberGenerated;
        globalSignals.OnCreatureFeedRequest += HandleCreatureFeedRequest;
        globalSignals.OnAreasToCleanRequest += HandleAreasToCleanRequest;
        globalSignals.OnCreatureFeedRequestSatisfied += HandleCreatureFeedRequestSatisfied;
        globalSignals.OnCreatureCleanRequestSatisfied += HandleCreatureCleanRequestSatisfied;
    }

    public override void _ExitTree()
    {
        globalSignals.OnEmployeeNumberGenerated -= HandleEmployeeNumberGenerated;
        globalSignals.OnCreatureFeedRequest -= HandleCreatureFeedRequest;
        globalSignals.OnAreasToCleanRequest -= HandleAreasToCleanRequest;
        globalSignals.OnCreatureFeedRequestSatisfied -= HandleCreatureFeedRequestSatisfied;
        globalSignals.OnCreatureCleanRequestSatisfied -= HandleCreatureCleanRequestSatisfied;
    }

    public void UpdateProgressBars(float newHunger, float newHappiness, float newCleanliness, float newTimeLeft)
    {
        hungerProgressBar.Value = newHunger;
        happinessProgressBar.Value = newHappiness;
        cleanlinessProgressBar.Value = newCleanliness;
        timeLeftText.Text = Mathf.RoundToInt(newTimeLeft).ToString();
    }

    private void HandleEmployeeNumberGenerated(int[] employeeNumber)
    {
        employeeNumberLabelNode.Text = string.Join("",employeeNumber);
    }

    private void HandleCreatureFeedRequest(Dictionary<E_IngredientList, bool> dictionary)
    {
        foreach (KeyValuePair<E_IngredientList, bool> keyValuePair in dictionary)
        {
            FoodRequest newFoodRequest = (FoodRequest)foodRequestScene.Instantiate();
            newFoodRequest.AssignLabelValues(keyValuePair.Key, keyValuePair.Value);
            foodRequestContainerNode.AddChild(newFoodRequest);
        }
    }

    private void HandleAreasToCleanRequest(Dictionary<E_AreasToClean, bool> dictionary)
    {
        foreach (KeyValuePair<E_AreasToClean, bool> keyValuePair in dictionary)
        {
            AreaCleanRequest newAreaCleanRequest = (AreaCleanRequest)areaCleanRequestScene.Instantiate();
            newAreaCleanRequest.AssignLabelValues(keyValuePair.Key, keyValuePair.Value);
            areaCleanRequestContainerNode.AddChild(newAreaCleanRequest);
        }
    }

    private void ClearRequestFromScreen(VBoxContainer containerToClear)
    {
        for (int i = 1; i < containerToClear.GetChildCount(); i++)
        {
            containerToClear.GetChild(i).QueueFree();
        }
    }

    private void HandleCreatureFeedRequestSatisfied(bool isSatisfied)
    {
        if (isSatisfied)
        {
            ClearRequestFromScreen(foodRequestContainerNode);
        }
    }

    private void HandleCreatureCleanRequestSatisfied(bool isSatisfied)
    {
        if (isSatisfied)
        {
            ClearRequestFromScreen(areaCleanRequestContainerNode);
        }
    }
}
