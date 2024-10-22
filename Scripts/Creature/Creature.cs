using Godot;
using System;

public partial class Creature : Node3D
{
    [ExportCategory("Required Nodes")]
    [Export] private MeshInstance3D eyeMeshNode = null;
    [Export] private AnimationPlayer animPlayerNode = null;
    [Export] private AudioStreamPlayer3D creatureAudioBedNode = null;

    private Player playerNode = null;

    public override void _Ready()
    {
        playerNode = GetNode<Player>("/root/Player");
        animPlayerNode.Play("Armature|Look", -1, 0.2f, false);
    }

    public void ActivateCreatureAudioBed()
    {
        creatureAudioBedNode.Play();
    }
}
