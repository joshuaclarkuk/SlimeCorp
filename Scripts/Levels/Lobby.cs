using Godot;
using System;

public partial class Lobby : Node3D
{
    [ExportCategory("Intro Nodes")]
    [Export] private TitleCard titleCardNode = null;
    [Export] private BlackScreen blackScreenNode = null;

    [ExportCategory("Music Nodes")]
    [Export] private AudioStreamPlayer musicNode = null;

    [ExportCategory("Lobby Object Nodes")]

    private GlobalSignals globalSignals = null;
    private GlobalValues globalValues = null;

    public override void _Ready()
    {
        globalValues = GetNode<GlobalValues>("/root/GlobalValues");
        globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");

        SubscribeToEvents();
    }

    public override void _ExitTree()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        globalSignals.OnBlackScreenDisappeared += HandleBlackScreenDisappeared;
        globalSignals.OnGenerateEmployeeNumber += HandleGenerateEmployeeNumber;
    }

    private void UnsubscribeFromEvents()
    {
        globalSignals.OnBlackScreenDisappeared -= HandleBlackScreenDisappeared;
        globalSignals.OnGenerateEmployeeNumber -= HandleGenerateEmployeeNumber;
    }

    private void HandleBlackScreenDisappeared()
    {        
        // Show title card
        titleCardNode.UpdateTextAndDisplay("Orientation");
        musicNode.Play();
    }

    private void HandleGenerateEmployeeNumber()
    {
        globalValues.GenerateEmployeeNumber();
    }
}
