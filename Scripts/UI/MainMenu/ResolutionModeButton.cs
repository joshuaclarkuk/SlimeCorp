using Godot;
using Godot.Collections;
using System;

public partial class ResolutionModeButton : Control
{
    [ExportCategory("Required Nodes")]
    [Export] private OptionButton resolutionButtonNode = null;

    private static readonly Dictionary<string, Vector2I> ScreenResolutionDictionary = new Dictionary<string, Vector2I>
    {
        { "1152 x 648", new Vector2I(1152, 648) },
        { "1280 x 720", new Vector2I(1280, 720) },
        { "1920 x 1080", new Vector2I(1920, 1080) },
        { "2560 x 1440", new Vector2I(2560, 1440) }
    };

    public override void _Ready()
    {
        resolutionButtonNode.ItemSelected += HandleResolutionButtonItemSelected;
        AddResolutionItems();
    }

    public override void _ExitTree()
    {
        resolutionButtonNode.ItemSelected -= HandleResolutionButtonItemSelected;
    }

    private void AddResolutionItems()
    {
        foreach (string resolution in ScreenResolutionDictionary.Keys)
        {
            resolutionButtonNode.AddItem(resolution);
        }
    }

    private void HandleResolutionButtonItemSelected(long index)
    {
        string selectedResolution = resolutionButtonNode.GetItemText((int)index);

        if (ScreenResolutionDictionary.TryGetValue(selectedResolution, out Vector2I resolution))
        {
            DisplayServer.WindowSetSize(resolution);
        }
        else
        {
            GD.PrintErr($"Resolution '{selectedResolution}' not found in the dictionary.");
        }
    }
}
