using Godot;
using System;

public partial class PoisonInjectorPickupStation : Station
{
    [ExportCategory("Required Nodes")]
    [Export] private CollisionShape3D collisionShapeNode = null;
    [Export] private MeshInstance3D injectorMeshNode = null;
    [Export] private Control pickupUINode = null;

    private Node3D poisonInjectorStationNode = null;

    private bool hasPickedUpPoisonInjector = false;

    public override void _Ready()
    {
        base._Ready();

        globalSignals.OnPlayerHasSupervisorCard += HandlePlayerHasSupervisorCard;

        DisableStation();
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        globalSignals.OnPlayerHasSupervisorCard -= HandlePlayerHasSupervisorCard;
    }

    public override void EnterStation()
    {
        base.EnterStation();

        pickupUINode.Visible = true;
    }

    public override void ExitStation()
    {
        base.ExitStation();

        if (!hasPickedUpPoisonInjector)
        {
            hasPickedUpPoisonInjector = true;
            injectorMeshNode.Visible = false;
            globalValues.SetHasPoisonInjector(true);
            GD.Print("Has picked up poison injector");
        }

        pickupUINode.Visible = false;
    }

    private void EnableStation()
    {
        Visible = true;
        collisionShapeNode.Disabled = false;
    }

    private void DisableStation()
    {
        Visible = false;
        pickupUINode.Visible = false;
        collisionShapeNode.Disabled = true;
    }

    private void HandlePlayerHasSupervisorCard()
    {
        EnableStation();
    }

    protected override void HandleButtonDisengaged(int buttonIndex)
    {
        throw new NotImplementedException();
    }

    protected override void HandleButtonEngaged(int buttonIndex)
    {
        throw new NotImplementedException();
    }
}
