using Godot;
using System;

public partial class GenerateEmployeeCardEvent : WorldEvent
{
    [ExportCategory("Required Nodes")]
    [Export] private Area3D spawnColliderNode = null;
    [Export] private Label3D receptionWrittenNoteNode = null;
    [Export] private Marker3D employeeCardSpawnLocationNode = null;
    [Export] private PackedScene employeeCardPickupScene = null;

    [ExportCategory("Reception Notes")]
    [Export(PropertyHint.MultilineText)] private string note1 = string.Empty;
    [Export(PropertyHint.MultilineText)] private string note2 = string.Empty;

    private bool hasSpawned = false;

    public override void _Ready()
    {
        base._Ready();
        SubscribeToEvents();

        // Initialise note
        receptionWrittenNoteNode.Text = note1;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        UnsubscribeFromEvents();
    }

    public override void SubscribeToEvents()
    {
        spawnColliderNode.BodyEntered += HandleSpawnColliderBodyEntered;
    }

    public override void UnsubscribeFromEvents()
    {
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
        GenerateEmployeeNumber();
        // Spawn card
        EmployeeCardPickup pickup = (EmployeeCardPickup)employeeCardPickupScene.Instantiate();
        AddChild(pickup);
        pickup.Position = employeeCardSpawnLocationNode.Position;

        receptionWrittenNoteNode.Text = note2;
        spawnColliderNode.GetChild<CollisionShape3D>(0).Disabled = true;
        hasSpawned = true;
    }

    private void GenerateEmployeeNumber()
    {
        // Generate employee number here
        Random random = new Random();
        int[] employeeNumber = new int[4];
        for (int i = 0; i < employeeNumber.Length; i++)
        {
            employeeNumber[i] = random.Next(0, 10);
        }
        globalSignals.RaiseGenerateEmployeeNumber(employeeNumber);

        GD.Print($"Generated employee number: {string.Join("", employeeNumber)} vs Official employee number: {string.Join("", globalValues.EmployeeNumber)}");
    }
}
