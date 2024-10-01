using Godot;
using System;
using System.Collections.Generic;

public partial class DebugUI : Control
{
    [ExportCategory("Required Nodes")]
    [Export] private Label employeeNumberLabelNode = null;
    [Export] private VBoxContainer foodRequestContainerNode = null;

    [ExportCategory("Scenes To Instantiate")]
    [Export] private PackedScene foodRequestScene = null;

    private GlobalSignals globalSignals = null;

    public override void _Ready()
    {
        globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");
        globalSignals.OnEmployeeNumberGenerated += HandleEmployeeNumberGenerated;
        globalSignals.OnCreatureFeedRequest += HandleCreatureFeedRequest;
    }

    public override void _ExitTree()
    {
        globalSignals.OnEmployeeNumberGenerated -= HandleEmployeeNumberGenerated;
        globalSignals.OnCreatureFeedRequest -= HandleCreatureFeedRequest;
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
}
