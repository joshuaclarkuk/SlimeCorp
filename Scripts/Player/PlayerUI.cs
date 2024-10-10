using Godot;
using System;

public partial class PlayerUI : Control
{
    [ExportCategory("Required Nodes")]
    [Export] public HBoxContainer NewEmailContainerNode { get; private set; } = null;

    private GlobalSignals globalSignals;

    public override void _Ready()
    {
        globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");
        globalSignals.OnEmailReceived += HandleEmailReceived;
        globalSignals.OnEmailsRead += HandleEmailsRead;
    }

    public override void _ExitTree()
    {
        globalSignals.OnEmailReceived -= HandleEmailReceived;
        globalSignals.OnEmailsRead -= HandleEmailsRead;
    }

    private void HandleEmailReceived(ComputerItemResource resource)
    {
        NewEmailContainerNode.Visible = true;
    }

    private void HandleEmailsRead()
    {
        NewEmailContainerNode.Visible = false;
    }
}
