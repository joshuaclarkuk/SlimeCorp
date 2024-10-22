using Godot;
using System;

public partial class PlayerUI : Control
{
    [ExportCategory("Required Nodes")]
    [Export] public HBoxContainer NewEmailContainerNode { get; private set; } = null;
    [Export] private Label interactLabelNode = null;

    private GlobalSignals globalSignals;

    private bool isClockedIn = false;

    public override void _Ready()
    {
        globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");
        globalSignals.OnEmailReceived += HandleEmailReceived;
        globalSignals.OnEmailsRead += HandleEmailsRead;
        globalSignals.OnPlayerClockedIn += HandlePlayerClockedIn;
        globalSignals.OnPlayerClockedOut += HandlePlayerClockedOut;
        globalSignals.OnPlayerEnterStationCollider += HandlePlayerEnterStationCollider;
        globalSignals.OnPlayerExitStationCollider += HandlePlayerExitStationCollider;
        globalSignals.OnPlayerInteractWithStation += HandlePlayerInteractWithStation;

        interactLabelNode.Visible = false;
    }

    public override void _ExitTree()
    {
        globalSignals.OnEmailReceived -= HandleEmailReceived;
        globalSignals.OnEmailsRead -= HandleEmailsRead;
        globalSignals.OnPlayerClockedIn -= HandlePlayerClockedIn;
        globalSignals.OnPlayerClockedOut -= HandlePlayerClockedOut;
        globalSignals.OnPlayerEnterStationCollider -= HandlePlayerEnterStationCollider;
        globalSignals.OnPlayerExitStationCollider -= HandlePlayerExitStationCollider;
        globalSignals.OnPlayerInteractWithStation -= HandlePlayerInteractWithStation;
    }

    private void HandlePlayerClockedIn()
    {
        isClockedIn = true;
    }

    private void HandlePlayerClockedOut()
    {
        isClockedIn = false;
    }

    private void HandleEmailReceived(ComputerItemResource resource)
    {
        NewEmailContainerNode.Visible = true;
    }

    private void HandleEmailsRead()
    {
        NewEmailContainerNode.Visible = false;
    }

    private void HandlePlayerEnterStationCollider(E_StationType type)
    {
        // If player is not clocked in, allow only CLOCKOUT or COMPUTER interactions
        if (!isClockedIn)
        {
            if (type == E_StationType.CLOCK_IN_STATION || type == E_StationType.COMPUTER_STATION)
            {
                interactLabelNode.Visible = true;
            }
            return;
        }

        interactLabelNode.Visible = true;
    }

    private void HandlePlayerExitStationCollider(E_StationType type)
    {
        interactLabelNode.Visible = false;
    }

    private void HandlePlayerInteractWithStation(E_StationType type)
    {
        interactLabelNode.Visible = false;
    }
}
