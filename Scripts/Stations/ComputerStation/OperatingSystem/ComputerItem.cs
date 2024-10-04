using Godot;
using System;

public abstract partial class ComputerItem : PanelContainer
{
    [ExportCategory("Required Nodes")]
    [Export] protected Label titleLabelNode = null;
    [Export] protected Label bodyLabelNode = null;
    [Export] protected Label bylineLabelNode = null;
    [Export] protected Label dateReceivedLabelNode = null;

    protected virtual string ItemTitle { get; set; } = string.Empty;
    protected virtual string ItemBody { get; set; } = string.Empty;
    protected virtual string ItemByline { get; set; } = string.Empty;
    protected virtual string ItemDateReceived { get; set; } = string.Empty;
    protected virtual bool HasBeenRead { get; set; } = false;

    public abstract void UpdateStringsFromResource(ComputerItemResource resource);
}
