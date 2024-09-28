using Godot;
using System;

public abstract partial class Station : Node
{
    [ExportCategory("Required Nodes")]
    [Export] private Label3D debugLabel = null;

    [ExportCategory("Station Type")]
    [Export] public E_StationType StationType { get; private set; }

    protected GlobalSignals globalSignals = null;
    private Area3D interactColliderNode = null;

    public override void _Ready()
    {
        globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");
        interactColliderNode = GetNode<Area3D>("InteractCollider");

        // Set debug label
        debugLabel.Text = StationType.ToString();

        interactColliderNode.BodyEntered += HandleInteractColliderNodeAreaEntered;
        interactColliderNode.BodyExited += HandleInteractColliderNodeAreaExited;
    }

    public override void _ExitTree()
    {
        interactColliderNode.BodyEntered -= HandleInteractColliderNodeAreaEntered;
        interactColliderNode.BodyExited -= HandleInteractColliderNodeAreaExited;
    }

    public abstract override void _UnhandledInput(InputEvent @event);

    public void HandleInteractColliderNodeAreaEntered(Node3D body)
    {
        if (body is Player)
        {
            globalSignals.RaisePlayerEnterStationCollider(StationType);
        }        
    }

    private void HandleInteractColliderNodeAreaExited(Node3D body)
    {
       if (body is Player)
        {
            globalSignals.RaisePlayerExitStationCollider(StationType);
        }
    }
}