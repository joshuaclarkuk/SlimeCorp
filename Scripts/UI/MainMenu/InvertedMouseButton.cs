using Godot;
using System;

public partial class InvertedMouseButton : Control
{
    [ExportCategory("Required Nodes")]
    [Export] private OptionButton optionButtonNode = null;

    private static readonly string[] InvertedMouseArray = { "Normal", "Inverted" };

    private Options options;

    public override void _Ready()
    {
        options = GetNode<Options>("/root/Options");

        optionButtonNode.ItemSelected += HandleOptionButtonItemSelected;
        AddInvertedMouseItems();
    }

    private void AddInvertedMouseItems()
    {
        foreach (string mouseSetting in InvertedMouseArray)
        {
            optionButtonNode.AddItem(mouseSetting);
        }
    }

    private void HandleOptionButtonItemSelected(long index)
    {
        switch (index)
        {
            case 0:
                options.SetIsInverted(false);
                break;
            case 1:
                options.SetIsInverted(true);
                break;
            default:
                options.SetIsInverted(false);
                break;
        }
    }
}
