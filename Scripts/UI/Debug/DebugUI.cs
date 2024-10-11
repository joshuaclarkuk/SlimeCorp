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
    [Export] private Label slimeRequiredLabelNode = null;
    [Export] private ProgressBar slimeCollectedProgressBarNode = null;
    [Export] private Label collectedSlimeTotalNode = null;

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

    public void UpdateCurrentSlimeProgressBar(float newSlimeAmount)
    {
        slimeCollectedProgressBarNode.Value = newSlimeAmount;
    }

    public void UpdateTotalSlimeCollectedProgressBar(float dailySlimeTotal, float totalSlimeRequested)
    {
        collectedSlimeTotalNode.Text = dailySlimeTotal.ToString();
        slimeRequiredLabelNode.Text = totalSlimeRequested.ToString();
        slimeCollectedProgressBarNode.Value = totalSlimeRequested / dailySlimeTotal * 100.0f;

        // Update daily slime total while accounting for potential div by 0 error at the beginning of the day
        if (dailySlimeTotal == 0.0f || float.IsInfinity(totalSlimeRequested / dailySlimeTotal) || float.IsNaN(totalSlimeRequested / dailySlimeTotal))
        {
            slimeCollectedProgressBarNode.Value = 0.0f; // Set to 0 if the division is invalid
        }
        else
        {
            slimeCollectedProgressBarNode.Value = totalSlimeRequested / dailySlimeTotal * 100.0f;
        }
    }

    public void UpdateFoodRequestList(List<E_IngredientList> list)
    {
        // Clear existing list
        Godot.Collections.Array<Node> childNodes = foodRequestContainerNode.GetChildren();

        for (int i = 1; i < foodRequestContainerNode.GetChildCount();  i++)
        {
            childNodes[i].QueueFree();
        }

        // Create new list with each request
        foreach(E_IngredientList ingredient in list)
        {
            FoodRequest newFoodRequest = (FoodRequest)foodRequestScene.Instantiate();
            newFoodRequest.AssignLabelValues(ingredient);
            foodRequestContainerNode.AddChild(newFoodRequest);
        }
    }

    public void UpdateCleaningRequestList(List<E_AreasToClean> list)
    {
        // Clear existing list
        Godot.Collections.Array<Node> childNodes = areaCleanRequestContainerNode.GetChildren();

        for (int i = 1; i < areaCleanRequestContainerNode.GetChildCount(); i++)
        {
            childNodes[i].QueueFree();
        }

        // Create new list with each request
        foreach (E_AreasToClean area in list)
        {
            AreaCleanRequest newAreaCleanRequest = (AreaCleanRequest)areaCleanRequestScene.Instantiate();
            newAreaCleanRequest.AssignLabelValues(area);
            areaCleanRequestContainerNode.AddChild(newAreaCleanRequest);
        }
    }

    private void SubscribeToEvents()
    {
        globalSignals.OnGenerateEmployeeNumber += HandleGenerateEmployeeNumber;
    }

    private void UnsubscribeFromEvents()
    {
        globalSignals.OnGenerateEmployeeNumber -= HandleGenerateEmployeeNumber;
    }

    private void HandleGenerateEmployeeNumber(int[] employeeNumber)
    {
        employeeNumberLabelNode.Text = string.Join("",employeeNumber);
    }
}
