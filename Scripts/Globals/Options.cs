using Godot;
using System;

public partial class Options : Node
{
    // DISPLAY VARIABLES
    public int WindowModeIndex { get; private set; } = 0;
    public int ResolutionIndex { get; private set; } = 0;

    // CONTROL VARIABLES
    public bool IsInverted { get; private set; } = false;

    public void SetIsInverted(bool isInverted)
    {
        IsInverted = isInverted;
    }
}