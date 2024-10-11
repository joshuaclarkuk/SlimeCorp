using Godot;
using System;

public partial class AreaCleanRequest : HBoxContainer
{
    [ExportCategory("Required Nodes")]
    [Export] private Label areaLabelNode = null;

    public void AssignLabelValues(E_AreasToClean areaName)
    {
        areaLabelNode.Text = areaName.ToString();
    }
}
