using Godot;
using System;

public partial class Options : Node
{
    // DISPLAY VARIABLES
    public int WindowModeIndex { get; private set; } = 0;
    public int ResolutionIndex { get; private set; } = 0;

    // CONTROL VARIABLES
    public bool IsInverted { get; private set; } = true;

    // DISPLAY EVENTS
    public event Action<int> OnWindowModeSet;
    public event Action<int> OnResolutionSet;

    // CONTROL EVENTS
    public event Action<bool> OnInvertToggled;

    public override void _Ready()
    {
        SubscribeToEvents();
    }

    public override void _ExitTree()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        // DISPLAY EVENTS
        OnWindowModeSet += HandleWindowModeSet;
        OnResolutionSet += HandleResolutionSet;

        // CONTROL EVENTS
        OnInvertToggled += HandleInvertToggled;
    }

    private void UnsubscribeFromEvents()
    {
        OnWindowModeSet -= HandleWindowModeSet;
    }

    private void HandleWindowModeSet(int modeIndex)
    {
        WindowModeIndex = modeIndex;
    }

    private void HandleResolutionSet(int resolutionIndex)
    {
        ResolutionIndex = resolutionIndex;
    }

    private void HandleInvertToggled(bool isInverted)
    {
        IsInverted = isInverted;
    }
}