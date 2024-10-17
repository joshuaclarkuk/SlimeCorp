using Godot;
using System;

public partial class EndScreen : Control
{
    [ExportCategory("Required Nodes")]
    [Export] private Label endingTextNode = null;

    [ExportCategory("Ending Text")]
    [Export(PropertyHint.MultilineText)] private string firedEndingText = null;
    [Export(PropertyHint.MultilineText)] private string playerWinEndingText = null;
    [Export(PropertyHint.MultilineText)] private string creatureWinEndingText = null;

    private GlobalValues globalValues = null;
    private Player player = null;

    public override void _Ready()
    {
        globalValues = GetNode<GlobalValues>("/root/GlobalValues");
        player = GetNode<Player>("/root/Player");

        player.QueueFree();
        PlayEndingScroll();
    }

    private void PlayEndingScroll()
    {
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
}
