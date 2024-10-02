using Godot;
using System;

public partial class AreaCleanRequest : HBoxContainer
{
    [ExportCategory("Required Nodes")]
    [Export] private Label areaLabelNode = null;
    [Export] private Label requestedLabelNode = null;

    public void AssignLabelValues(E_AreasToClean areaName, bool requestedValue)
    {
        areaLabelNode.Text = areaName.ToString();
        requestedLabelNode.Text = requestedValue.ToString();
    }
}
