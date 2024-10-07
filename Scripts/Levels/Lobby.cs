using Godot;
using System;

public partial class Lobby : Node3D
{
    [ExportCategory("Player Start")]
    [Export] private Marker3D playerStartNode = null;

    [ExportCategory("Intro Nodes")]
    [Export] private TitleCard titleCardNode = null;
    [Export] private BlackScreen blackScreenNode = null;

    [ExportCategory("Music Nodes")]
    [Export] private AudioStreamPlayer musicNode = null;

    private GlobalSignals globalSignals = null;
    private GlobalValues globalValues = null;
    private Player player = null;

    public override void _Ready()
    {
        globalValues = GetNode<GlobalValues>("/root/GlobalValues");
        globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");
        player = GetNode<Player>("/root/Player");

        // Set player to player start
        player.GlobalTransform = playerStartNode.GlobalTransform;

        SubscribeToEvents();
    }

    public override void _ExitTree()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        globalSignals.OnBlackScreenDisappeared += HandleBlackScreenDisappeared;
    }

    private void UnsubscribeFromEvents()
    {
        globalSignals.OnBlackScreenDisappeared -= HandleBlackScreenDisappeared;
    }

    private void HandleBlackScreenDisappeared()
    {        
        // Show title card
        titleCardNode.UpdateTextAndDisplay("Orientation");
        musicNode.Play();
    }
}
