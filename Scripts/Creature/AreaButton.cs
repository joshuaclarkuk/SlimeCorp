using Godot;
using System;

public partial class AreaButton : CsgBox3D
{
    public E_AreasToClean AssignedArea { get; private set; }

    [ExportCategory("Required Nodes")]
    [Export] private Label3D debugLabelNode = null;

    [ExportCategory("Materials")]
    [Export] private Material defaultMaterial = null;
    [Export] private Material lightUpMaterial = null;

    public override void _Ready()
    {
        DeactivateButton();
    }

    public void AssignEnumValue(E_AreasToClean area)
    {
        AssignedArea = area;
        debugLabelNode.Text = AssignedArea.ToString();
    }

    public void ActivateButton()
    {
        Material = lightUpMaterial;
    }

    public void DeactivateButton()
    {
        Material = defaultMaterial;
    }
}
