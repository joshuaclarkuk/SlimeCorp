using Godot;
using System;

public partial class EndScreen : Control
{
    [ExportCategory("Required Nodes")]
    [Export] private Label endingTextNode = null;
    [Export] private Button quitButtonNode = null;

    [ExportCategory("Ending Text")]
    [Export(PropertyHint.MultilineText)] private string firedEndingText = null;
    [Export(PropertyHint.MultilineText)] private string playerWinEndingText = null;
    [Export(PropertyHint.MultilineText)] private string creatureWinEndingText = null;

    private GlobalValues globalValues = null;
    private Player player = null;

    private float percentageIncrement = 0.01f;
    private float elapsedTime = 0.0f;
    private float updateInterval = 0.02f; // Time in seconds between updates

    public override void _Ready()
    {
        globalValues = GetNode<GlobalValues>("/root/GlobalValues");
        player = GetNode<Player>("/root/Player");

        // Link quit button signals
        quitButtonNode.Pressed += HandleQuitButtonPressed;

        player.QueueFree();
        PlayEndingScroll();
    }

    public override void _Process(double delta)
    {
        RevealText(delta);
    }

    private void PlayEndingScroll()
    {
        endingTextNode.VisibleRatio = 0.0f;

        switch (globalValues.EndState)
        {
            case E_EndState.NONE:
                GD.PrintErr("No valid ending set");
                break;
            case E_EndState.FIRED:
                GD.Print("Fired ending");
                endingTextNode.Text = firedEndingText;
                break;
            case E_EndState.PLAYER_WIN:
                GD.Print("Player won");
                endingTextNode.Text = playerWinEndingText;
                break;
            case E_EndState.CREATURE_WIN:
                GD.Print("Creature won");
                endingTextNode.Text = creatureWinEndingText;
                break;
        }
    }

    private void RevealText(double delta)
    {
        elapsedTime += (float)delta;

        if (elapsedTime >= updateInterval)
        {
            elapsedTime = 0.0f;

            if (endingTextNode.VisibleRatio < 1.0f)
            {
                endingTextNode.VisibleRatio += percentageIncrement;

                if (endingTextNode.VisibleRatio > 1.0f)
                {
                    endingTextNode.VisibleRatio = 1.0f;  // Clamp the value to 1.0
                }
            }
            else
            {
                // Stop the process loop when the text is fully revealed
                SetProcess(false);
            }
        }
    }

    private void HandleQuitButtonPressed()
    {
        GetTree().Quit();
    }
}
