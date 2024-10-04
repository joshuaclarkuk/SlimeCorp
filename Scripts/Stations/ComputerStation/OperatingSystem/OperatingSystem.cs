using Godot;
using System;

public partial class OperatingSystem : Control
{
    [ExportCategory("Item Spawners")]
    [Export] private EmailItemSpawner emailItemSpawnerNode = null;
    [Export] private NewsItemSpawner newsItemSpawnerNode = null;

    [ExportCategory("Resources")]
    [Export] private ComputerItemResource[] emailItemResources;
    [Export] private ComputerItemResource[] newsItemResources;

    private GlobalSignals globalSignals = null;

    public override void _Ready()
    {
        globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");
        globalSignals.OnEmailReceived += HandleEmailReceived;
        globalSignals.OnNewsArticleReceived += HandleNewsArticleReceived;
    }

    public override void _ExitTree()
    {
        globalSignals.OnEmailReceived -= HandleEmailReceived;
        globalSignals.OnNewsArticleReceived -= HandleNewsArticleReceived;
    }

    private void HandleEmailReceived(ComputerItemResource resource)
    {
        emailItemSpawnerNode.AddNewItemToScreen(resource);
    }

    private void HandleNewsArticleReceived(ComputerItemResource resource)
    {
        newsItemSpawnerNode.AddNewItemToScreen(resource);
    }
}
