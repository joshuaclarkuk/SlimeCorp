using Godot;
using System;

public partial class EmployeeCardEvent : Node3D
{
    [ExportCategory("Required Nodes")]
    [Export] private Area3D spawnColliderNode = null;
    [Export] private Area3D pickUpCardColliderNode = null;
    [Export] private MeshInstance3D cardMeshNode = null;
    [Export] private Label3D receptionWrittenNoteNode = null;

    [ExportCategory("Reception Notes")]
    [Export(PropertyHint.MultilineText)] private string note1 = string.Empty;
    [Export(PropertyHint.MultilineText)] private string note2 = string.Empty;

    private GlobalSignals globalSignals = null;

    public override void _Ready()
    {
        // Get global signals to generate employee number
        globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");

        // Subscribe to events
        spawnColliderNode.BodyEntered += HandleSpawnColliderBodyEntered;

        // Initialise colliders
        TogglePickUpCardCollision(false);
        receptionWrittenNoteNode.Text = note1;
    }

    public override void _ExitTree()
    {
        // Unsubscribe from events
        spawnColliderNode.BodyEntered -= HandleSpawnColliderBodyEntered;
    }

    private void HandleSpawnColliderBodyEntered(Node3D body)
    {
        if (body is Player)
        {
            GD.Print("Employee card spawned");
            this.CallDeferred(nameof(DeferredCardSpawnActions)); // Have to defer to be able to disable the collision object that's registering the collision
        }
    }

    private void DeferredCardSpawnActions()
    {
        TogglePickUpCardCollision(true);
        receptionWrittenNoteNode.Text = note2;
        spawnColliderNode.GetChild<CollisionShape3D>(0).Disabled = true;
        globalSignals.RaiseGenerateEmployeeNumber();
    }

    private void TogglePickUpCardCollision(bool isActive)
    {
        cardMeshNode.Visible = isActive;
        pickUpCardColliderNode.GetChild<CollisionShape3D>(0).Disabled = !isActive;
    }
}
