using Godot;
using System;

public partial class CleaningStation : Station
{
    public override void EnterStation()
    {
        GD.Print($"Calling EnterStation method on {Name}");
    }

    public override void ExitStation()
    {
        GD.Print($"Calling ExitStation method on {Name}");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);
        GD.Print($"{Name}: Unhandled Input active");
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
                // Execute behavior for button 1
                break;
            case 3:
                // Execute behavior for button 1
                break;
            case 4:
                // Execute behavior for button 1
                break;
            case 5:
                // Execute behavior for button 1
                break;
            case 6:
                // Execute behavior for button 1
                break;
            case 7:
                // Execute behavior for button 1
                break;
            case 8:
                // Execute behavior for button 1
                break;
            case 9:
                // Execute behavior for button 1
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
                // Execute behavior for button 1
                break;
            case 3:
                // Execute behavior for button 1
                break;
            case 4:
                // Execute behavior for button 1
                break;
            case 5:
                // Execute behavior for button 1
                break;
            case 6:
                // Execute behavior for button 1
                break;
            case 7:
                // Execute behavior for button 1
                break;
            case 8:
                // Execute behavior for button 1
                break;
            case 9:
                // Execute behavior for button 1
                break;
            default:
                GD.Print("Unhandled button press");
                break;
        }
    }
}
