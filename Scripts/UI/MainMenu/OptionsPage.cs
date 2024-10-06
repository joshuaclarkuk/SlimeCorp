using Godot;
using System;

public partial class OptionsPage : Control
{
    [ExportCategory("Required Nodes")]
    [Export] private Button returnToMenuButtonNode = null;

    public event Action OnReturnToMenuButtonClick;

    public override void _Ready()
    {
        // Stop options menu being clickable on start
        SetProcess(false);

        SubscribeToEvents();
    }

    public override void _ExitTree()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        returnToMenuButtonNode.Pressed += HandleReturnToMenuButtonPressed;
    }

    private void UnsubscribeFromEvents()
    {
        returnToMenuButtonNode.Pressed -= HandleReturnToMenuButtonPressed;
    }

    private void HandleReturnToMenuButtonPressed()
    {
        SetProcess(false);
        OnReturnToMenuButtonClick.Invoke();
    }
}
