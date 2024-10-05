using Godot;
using System;

public partial class Lobby : Node3D
{
    [ExportCategory("Intro Nodes")]
    [Export] private TitleCard titleCardNode = null;
    [Export] private BlackScreen blackScreenNode = null;

    [ExportCategory("Music Nodes")]
    [Export] private AudioStreamPlayer musicNode = null;

    private GlobalSignals globalSignals;

    public override void _Ready()
    {
        globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");
        SubscribeToEvents();

        PlayIntroSequence();
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

    private void PlayIntroSequence()
    {

    }

    private void HandleBlackScreenDisappeared()
    {        
        // Show title card
        titleCardNode.UpdateTextAndDisplay("Lobby");
        musicNode.Play();

    }
}
