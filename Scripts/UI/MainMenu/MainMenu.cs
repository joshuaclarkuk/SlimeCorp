using Godot;
using System;

public partial class MainMenu : Control
{
    [ExportCategory("Start Menu Buttons")]
    [Export] private Button startGameButtonNode = null;
    [Export] private Button optionsButtonNode = null;
    [Export] private Button quitGameButtonNode = null;

    [ExportCategory("Required Nodes")]
    [Export] private PackedScene firstLevelToLoad = null;
    [Export] private Control startPageNode = null;
    [Export] private OptionsPage optionsPageNode = null;

    public override void _Ready()
    {
        SubscribeToEvents();

        // Initialise pages
        optionsPageNode.Visible = false;
        startPageNode.Visible = true;

        // Set mouse to visible
        Input.MouseMode = Input.MouseModeEnum.Visible;
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

        // OPTIONS MENU
        optionsPageNode.OnReturnToMenuButtonClick += HandleReturnToMenuButtonClick;
    }

    private void UnsubscribeFromEvents()
    {
        // START MENU
        startGameButtonNode.Pressed -= HandleStartGameButtonPressed;
        optionsButtonNode.Pressed -= HandleOptionsButtonPressed;
        quitGameButtonNode.Pressed -= HandleQuitGameButtonPressed;

        // OPTIONS MENU
        optionsPageNode.OnReturnToMenuButtonClick -= HandleReturnToMenuButtonClick;
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

        startPageNode.SetProcess(false);
        optionsPageNode.SetProcess(true);
    }

    private void HandleReturnToMenuButtonClick()
    {
        startPageNode.Visible = true;
        optionsPageNode.Visible = false;

        startPageNode.SetProcess(true);
        optionsPageNode.SetProcess(false);
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
