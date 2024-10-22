using Godot;
using System;

public partial class PauseMenu : Control
{
    [ExportCategory("Required Nodes")]
    [Export] private Button resumeGameButtonNode = null;
    [Export] private Button quitGameButtonNode = null;

    public override void _Ready()
    {
        resumeGameButtonNode.Pressed += HandleResumeGameButtonPressed;
        quitGameButtonNode.Pressed += HandleQuitGameButtonPressed;

        Visible = false;
        ProcessMode = ProcessModeEnum.WhenPaused;
    }

    public override void _ExitTree()
    {
        resumeGameButtonNode.Pressed -= HandleResumeGameButtonPressed;
        quitGameButtonNode.Pressed -= HandleQuitGameButtonPressed;
    }

    public void TogglePauseMenu(bool shouldPause)
    {
        if (shouldPause)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    private void PauseGame()
    {
        Visible = true;
        Input.MouseMode = Input.MouseModeEnum.Visible;
        GetTree().Paused = true;
    }

    private void ResumeGame()
    {
        Visible = false;
        Input.MouseMode = Input.MouseModeEnum.Captured;
        GetTree().Paused = false;
    }

    private void HandleResumeGameButtonPressed()
    {
        ResumeGame();
    }

    private void HandleQuitGameButtonPressed()
    {
        GetTree().Quit();
    }
}
