using Godot;
using System;

public partial class EmployeeCardEvent : Node3D
{
    [ExportCategory("Required Nodes")]
    [Export] private Area3D spawnColliderNode = null;
    [Export] private Label3D receptionWrittenNoteNode = null;
    [Export] private Marker3D employeeCardSpawnLocationNode = null;
    [Export] private PackedScene employeeCardPickupScene = null;

    [ExportCategory("Reception Notes")]
    [Export(PropertyHint.MultilineText)] private string note1 = string.Empty;
    [Export(PropertyHint.MultilineText)] private string note2 = string.Empty;

    private GlobalSignals globalSignals = null;

    private bool hasSpawned = false;

    public override void _Ready()
    {
        // Get global signals to generate employee number
        globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");

        // Subscribe to events
        spawnColliderNode.BodyEntered += HandleSpawnColliderBodyEntered;

        // Initialise note
        receptionWrittenNoteNode.Text = note1;
    }

    public override void _ExitTree()
    {
        // Unsubscribe from events
        spawnColliderNode.BodyEntered -= HandleSpawnColliderBodyEntered;
    }

    private void HandleSpawnColliderBodyEntered(Node3D body)
    {
        if (hasSpawned) { return; }

        if (body is Player)
        {
            this.CallDeferred(nameof(DeferredCardSpawnActions)); // Have to defer to be able to disable the collision object that's registering the collision
        }
    }

    private void DeferredCardSpawnActions()
    {
        // Spawn card
        EmployeeCardPickup pickup = (EmployeeCardPickup)employeeCardPickupScene.Instantiate();
        AddChild(pickup);
        pickup.Position = employeeCardSpawnLocationNode.Position;

        receptionWrittenNoteNode.Text = note2;
        spawnColliderNode.GetChild<CollisionShape3D>(0).Disabled = true;
        globalSignals.RaiseGenerateEmployeeNumber();
        hasSpawned = true;
    }
}
