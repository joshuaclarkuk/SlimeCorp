using Godot;
using System;

public partial class MainMenu : Control
{
    [ExportCategory("Start Menu Buttons")]
    [Export] private Button startGameButtonNode = null;
    [Export] private Button optionsButtonNode = null;
    [Export] private Button quitGameButtonNode = null;

    [ExportCategory("Options Menu Buttons")]
    [Export] private CheckBox displayModeOptionsButtonNode = null;
    [Export] private Button backToMenuButtonNode = null;

    [ExportCategory("Required Nodes")]
    [Export] private PackedScene firstLevelToLoad = null;
    [Export] private Control startPageNode = null;
    [Export] private Control optionsPageNode = null;

    private bool isMaximised = false;

    public override void _Ready()
    {
        SubscribeToEvents();

        // Initialise pages
        optionsPageNode.Visible = false;
        startPageNode.Visible = true;
    }

    public override void _ExitTree()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        // START MENU
        startGameButtonNode.Pressed += HandleStartGameButtonPressed;
        optionsButtonNode.Pressed += HandleOptionsButtonPressed;
        quitGameButtonNode.Pressed += HandleQuitGameButtonPressed;
        backToMenuButtonNode.Pressed += HandleBackToMenuButtonPressed;

        // OPTIONS MENU
    }

    private void UnsubscribeFromEvents()
    {
        // START MENU
        startGameButtonNode.Pressed -= HandleStartGameButtonPressed;
        optionsButtonNode.Pressed -= HandleOptionsButtonPressed;
        quitGameButtonNode.Pressed -= HandleQuitGameButtonPressed;
        backToMenuButtonNode.Pressed -= HandleBackToMenuButtonPressed;

        // OPTIONS MENU
    }

    // START MENU
    private void HandleStartGameButtonPressed()
    {
        GetTree().ChangeSceneToPacked(firstLevelToLoad);
    }

    private void HandleOptionsButtonPressed()
    {
        startPageNode.Visible = false;
        optionsPageNode.Visible = true;
    }

    private void HandleQuitGameButtonPressed()
    {
        GetTree().Quit();
    }

    // OPTIONS MENU
    private void HandleBackToMenuButtonPressed()
    {
        optionsPageNode.Visible = false;
        startPageNode.Visible = true;
    }
}
