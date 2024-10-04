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
        SubscribeToSignals();

        // Set invisible on start
        Modulate = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        Visible = false;
    }

    public override void _ExitTree()
    {
        UnsubscribeFromSignals();
    }

    private void SubscribeToSignals()
    {
        globalSignals.OnEmailReceived += HandleEmailReceived;
        globalSignals.OnNewsArticleReceived += HandleNewsArticleReceived;
        globalSignals.OnPlayerInteractWithStation += HandlePlayerInteractWithStation;
        globalSignals.OnPlayerExitStation += HandlePlayerExitStation;
    }

    private void UnsubscribeFromSignals()
    {
        globalSignals.OnEmailReceived -= HandleEmailReceived;
        globalSignals.OnNewsArticleReceived -= HandleNewsArticleReceived;
        globalSignals.OnPlayerInteractWithStation -= HandlePlayerInteractWithStation;
        globalSignals.OnPlayerExitStation -= HandlePlayerExitStation;
    }

    private void FadeComputerScreen(float finalValue)
    {
        Tween fadeTween = CreateTween();
        fadeTween.Finished += HandleFadeTweenFinished;
        fadeTween.SetTrans(Tween.TransitionType.Sine);
        fadeTween.SetEase(Tween.EaseType.InOut);
        fadeTween.TweenProperty(this, "modulate:a", finalValue, 1.0f);

        if (finalValue > 0)
        {
            Visible = true;
        }
    }

    private void HandlePlayerInteractWithStation(E_StationType stationType)
    {
        Visible = true;
        if (stationType == E_StationType.COMPUTER) { FadeComputerScreen(1.0f); }
    }

    private void HandlePlayerExitStation(E_StationType stationType)
    {
        if (stationType == E_StationType.COMPUTER) { FadeComputerScreen(0.0f); }
    }

    private void HandleEmailReceived(ComputerItemResource resource)
    {
        emailItemSpawnerNode.AddNewItemToScreen(resource);
    }

    private void HandleNewsArticleReceived(ComputerItemResource resource)
    {
        newsItemSpawnerNode.AddNewItemToScreen(resource);
    }

    private void HandleFadeTweenFinished()
    {
        if (Modulate.A <= 0.0f)
        {
            Visible = false;
        }
    }
}
