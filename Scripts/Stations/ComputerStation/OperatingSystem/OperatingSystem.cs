using Godot;
using System;

public partial class OperatingSystem : Control
{
    [ExportCategory("Required Nodes")]
    [Export] private TabContainer tabContainerNode = null;

    [ExportCategory("Item Spawners")]
    [Export] private ToDoItemSpawner toDoItemSpawnerNode = null;
    [Export] private EmailItemSpawner emailItemSpawnerNode = null;
    [Export] private NewsItemSpawner newsItemSpawnerNode = null;

    [ExportCategory("Behaviour")]
    [Export] private float fadeDuration = 0.3f;

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
        globalSignals.OnToDoItemReceived += HandleToDoItemReceived;
        globalSignals.OnEmailReceived += HandleEmailReceived;
        globalSignals.OnNewsArticleReceived += HandleNewsArticleReceived;
        globalSignals.OnPlayerInteractWithStation += HandlePlayerInteractWithStation;
        globalSignals.OnPlayerExitStation += HandlePlayerExitStation;
        tabContainerNode.TabClicked += HandleTabClicked;
    }

    private void UnsubscribeFromSignals()
    {
        globalSignals.OnToDoItemReceived -= HandleToDoItemReceived;
        globalSignals.OnEmailReceived -= HandleEmailReceived;
        globalSignals.OnNewsArticleReceived -= HandleNewsArticleReceived;
        globalSignals.OnPlayerInteractWithStation -= HandlePlayerInteractWithStation;
        globalSignals.OnPlayerExitStation -= HandlePlayerExitStation;
        tabContainerNode.TabClicked -= HandleTabClicked;
    }

    private void FadeComputerScreen(float finalValue)
    {
        Tween fadeTween = CreateTween();
        fadeTween.Finished += HandleFadeTweenFinished;
        fadeTween.SetTrans(Tween.TransitionType.Sine);
        fadeTween.SetEase(Tween.EaseType.InOut);
        fadeTween.TweenProperty(this, "modulate:a", finalValue, fadeDuration);

        if (finalValue > 0)
        {
            Visible = true;
        }
    }

    private void HandlePlayerInteractWithStation(E_StationType stationType)
    {
        // Set active tab to be TO DO list
        tabContainerNode.CurrentTab = 0;

        // Register mouse events
        MouseFilter = MouseFilterEnum.Stop;

        if (stationType == E_StationType.COMPUTER) { FadeComputerScreen(1.0f); }
    }

    private void HandlePlayerExitStation(E_StationType stationType)
    {
        // Ignore mouse events
        MouseFilter = MouseFilterEnum.Ignore;

        if (stationType == E_StationType.COMPUTER) { FadeComputerScreen(0.0f); }
    }

    private void HandleToDoItemReceived(ComputerItemResource resource)
    {
        toDoItemSpawnerNode.AddNewItemToScreen(resource);
    }

    private void HandleEmailReceived(ComputerItemResource resource)
    {
        emailItemSpawnerNode.AddNewItemToScreen(resource);
    }

    private void HandleNewsArticleReceived(ComputerItemResource resource)
    {
        newsItemSpawnerNode.AddNewItemToScreen(resource);
    }

    private void HandleTabClicked(long tab)
    {
        // If email tab clicked, raise emails raid signal to turn off player email notification
        if (tab == 1)
        {
            globalSignals.RaiseEmailsRead();
        }
    }

    private void HandleFadeTweenFinished()
    {
        if (Modulate.A <= 0.0f)
        {
            Visible = false;
        }
    }
}
