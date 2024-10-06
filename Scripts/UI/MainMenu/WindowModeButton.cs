using Godot;
using System;

public partial class WindowModeButton : Control
{
    [ExportCategory("Required Nodes")]
    [Export] private OptionButton optionButtonNode = null;

    private static readonly string[] WindowModeArray = { "Full Screen", "Window Mode", "Borderless Window", "Borderless Full Screen" };

    public override void _Ready()
    {
        optionButtonNode.ItemSelected += HandleOptionButtonItemSelected;
        AddWindowModeItems();
    }

    public override void _ExitTree()
    {
        optionButtonNode.ItemSelected -= HandleOptionButtonItemSelected;
    }

    private void AddWindowModeItems()
    {
        foreach (string windowMode in WindowModeArray)
        {
            optionButtonNode.AddItem(windowMode);
        }
    }

    private void HandleOptionButtonItemSelected(long index)
    {
        switch (index)
        {
            case 0: // Full screen
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
                DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
                break;
            case 1: // Window Mode
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
                break;
            case 2: // Borderless Window
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, true);
                break;
            case 3: // Borderless Full Screen
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
                DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, true);
                break;
            default:
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
                DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
                break;
        }
    }
}
