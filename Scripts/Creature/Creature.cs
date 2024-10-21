using Godot;
using System;

public partial class Creature : Node3D
{
    [ExportCategory("Required Nodes")]
    [Export] private MeshInstance3D eyeMeshNode = null;
    [Export] private AnimationPlayer animPlayerNode = null;

    private Player playerNode = null;

    public override void _Ready()
    {
        playerNode = GetNode<Player>("/root/Player");
        animPlayerNode.Play("Armature|Look", -1, 0.2f, false);
    }

    //public override void _Process(double delta)
    //{
    //    eyeMeshNode.LookAt(playerNode.GlobalPosition);
    //}
}
