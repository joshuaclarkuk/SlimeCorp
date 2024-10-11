using Godot;
using System;

public partial class StationNeedsProgressBarComponent : Node3D
{
    [ExportCategory("Required Nodes")]
    [Export] private ProgressBar progressBar = null;

    public void SetProgressBarValue(float value)
    {
        progressBar.Value = value;
    }

    public void ResetProgressBar()
    {
        progressBar.Value = 0.0f;
    }
}
