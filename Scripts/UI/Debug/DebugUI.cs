using Godot;
using System;
using System.Collections.Generic;

public partial class DebugUI : Control
{
    [ExportCategory("Employee Number Nodes")]
    [Export] private Label employeeNumberLabelNode = null;

    [ExportCategory("Creature Needs Nodes")]
    [Export] private VBoxContainer foodRequestContainerNode = null;
    [Export] private VBoxContainer areaCleanRequestContainerNode = null;
    [Export] private ProgressBar hungerProgressBar = null;
    [Export] private ProgressBar happinessProgressBar = null;
    [Export] private ProgressBar cleanlinessProgressBar = null;
    [Export] private Label timeLeftText = null;

    [ExportCategory("Slime Collected Nodes")]
    [Export] private ProgressBar slimeCollectedProgressBarNode = null;

    [ExportCategory("Scenes To Instantiate")]
    [Export] private PackedScene foodRequestScene = null;
    [Export] private PackedScene areaCleanRequestScene = null;

    private GlobalSignals globalSignals = null;
    private GlobalValues globalValues = null;

    public override void _Ready()
    {
        // Get global signals and subscribe
        globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");

        // Get global values for employee number
        globalValues = GetNode<GlobalValues>("/root/GlobalValues");

        SubscribeToEvents();
    }

    public override void _ExitTree()
    {
        UnsubscribeFromEvents();
    }

    public void UpdateProgressBars(float newHunger, float newHappiness, float newCleanliness, float newTimeLeft)
    {
        hungerProgressBar.Value = newHunger;
        happinessProgressBar.Value = newHappiness;
        cleanlinessProgressBar.Value = newCleanliness;
        timeLeftText.Text = Mathf.RoundToInt(newTimeLeft).ToString();
    }

    public void UpdateSlimeProgressBar(float newSlimeAmount)
    {
        slimeCollectedProgressBarNode.Value = newSlimeAmount;
    }

    private void SubscribeToEvents()
    {
        globalSignals.OnCreatureFeedRequest += HandleCreatureFeedRequest;
        globalSignals.OnAreasToCleanRequest += HandleAreasToCleanRequest;
        globalSignals.OnCreatureFeedRequestSatisfied += HandleCreatureFeedRequestSatisfied;
        globalSignals.OnCreatureCleanRequestSatisfied += HandleCreatureCleanRequestSatisfied;
        globalSignals.OnGenerateEmployeeNumber += HandleGenerateEmployeeNumber;
    }

    private void UnsubscribeFromEvents()
    {
        globalSignals.OnCreatureFeedRequest -= HandleCreatureFeedRequest;
        globalSignals.OnAreasToCleanRequest -= HandleAreasToCleanRequest;
        globalSignals.OnCreatureFeedRequestSatisfied -= HandleCreatureFeedRequestSatisfied;
        globalSignals.OnCreatureCleanRequestSatisfied -= HandleCreatureCleanRequestSatisfied;
        globalSignals.OnGenerateEmployeeNumber -= HandleGenerateEmployeeNumber;
    }

    private void HandleGenerateEmployeeNumber(int[] employeeNumber)
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
