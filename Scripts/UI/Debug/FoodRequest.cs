using Godot;
using System;

public partial class FoodRequest : HBoxContainer
{
    [ExportCategory("Required Nodes")]
    [Export] private Label ingredientLabelNode = null;

    public void AssignLabelValues(E_IngredientList ingredientName)
    {
        ingredientLabelNode.Text = ingredientName.ToString();
    }
}