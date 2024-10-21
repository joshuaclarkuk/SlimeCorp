using Godot;
using System;

public partial class ComputerStation : Station
{
    [ExportCategory("Required Nodes")]
    [Export] private SpotLight3D spotlightNode = null;

    public override void _Ready()
    {
        base._Ready();

        globalSignals.OnBlackScreenDisappeared += HandleBlackScreenDisappeared;
        globalSignals.OnPlayerClockedIn += HandlePlayerClockedIn;
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        globalSignals.OnBlackScreenDisappeared -= HandleBlackScreenDisappeared;
        globalSignals.OnPlayerClockedIn -= HandlePlayerClockedIn;
    }

    public override void EnterStation()
    {
        base.EnterStation();

        GD.Print($"Calling EnterStation method on {Name}");
    }

    public override void ExitStation()
    {
        base.ExitStation();

        GD.Print($"Calling ExitStation method on {Name}");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput( @event );
    }

    protected override void HandleButtonEngaged(int buttonIndex)
    {
        GD.Print($"Button {buttonIndex} was pressed in {Name}.");

        switch (buttonIndex)
        {
            case 0:
                // Execute behavior for button 0
                break;
            case 1:
                // Execute behavior for button 1
                break;
            case 2:
                // Execute behavior for button 2
                break;
            case 3:
                // Execute behavior for button 3
                break;
            case 4:
                // Execute behavior for button 4
                break;
            case 5:
                // Execute behavior for button 5
                break;
            case 6:
                // Execute behavior for button 6
                break;
            case 7:
                // Execute behavior for button 7
                break;
            case 8:
                // Execute behavior for button 8
                break;
            case 9:
                // Execute behavior for button 9
                break;
            default:
                GD.Print("Unhandled button press");
                break;
        }
    }

    protected override void HandleButtonDisengaged(int buttonIndex)
    {
        GD.Print($"Button {buttonIndex} was released in FeedingStation.");

        switch (buttonIndex)
        {
            case 0:
                // Execute behavior for button 0
                break;
            case 1:
                // Execute behavior for button 1
                break;
            case 2:
                // Execute behavior for button 2
                break;
            case 3:
                // Execute behavior for button 3
                break;
            case 4:
                // Execute behavior for button 4
                break;
            case 5:
                // Execute behavior for button 5
                break;
            case 6:
                // Execute behavior for button 6
                break;
            case 7:
                // Execute behavior for button 7
                break;
            case 8:
                // Execute behavior for button 8
                break;
            case 9:
                // Execute behavior for button 9
                break;
            default:
                GD.Print("Unhandled button press");
                break;
        }
    }

    private void HandleBlackScreenDisappeared()
    {
        spotlightNode.Visible = true;
    }

    private void HandlePlayerClockedIn()
    {
        spotlightNode.Visible = false;
    }
}
