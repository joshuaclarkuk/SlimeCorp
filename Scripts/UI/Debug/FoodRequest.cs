using Godot;
using System;

public partial class FoodRequest : HBoxContainer
{
    [ExportCategory("Required Nodes")]
    [Export] private Label ingredientLabelNode = null;
    [Export] private Label requestedLabelNode = null;

    public void AssignLabelValues(E_IngredientList ingredientName, bool requestedValue)
    {
        ingredientLabelNode.Text = ingredientName.ToString();
        requestedLabelNode.Text = requestedValue.ToString();
    }
}