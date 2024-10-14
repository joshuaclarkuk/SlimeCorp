using Godot;
using System;

public partial class ClockOutStation : Station
{
    [ExportCategory("Required Nodes")]
    [Export] private Card punchCardNode = null;
    [Export] private CodeComponent codeComponent = null;

    // Card behaviour
    private bool cardInMachine = false;
    private bool clockedIn = false;

    public override void _Ready()
    {
        base._Ready();

        AssignDebugLabelValues();
    }

    public override void EnterStation()
    {
        base.EnterStation();

        // Link signals
        punchCardNode.OnCardTargetReached += HandleCardTargetReached;
        codeComponent.OnCorrectCodeEntered += HandleCorrectCodeEntered;

        GD.Print($"Calling EnterStation method on {Name}");
    }

    public override void ExitStation()
    {
        base.ExitStation();

        // Unlink signals
        punchCardNode.OnCardTargetReached -= HandleCardTargetReached;
        codeComponent.OnCorrectCodeEntered -= HandleCorrectCodeEntered;

        // Reset machine
        punchCardNode.ReturnToOriginalPosition();
        cardInMachine = false;

        GD.Print($"Calling ExitStation method on {Name}");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput( @event );

        if (isMouseClicked)
        {
            if (cardInMachine) { return; } // Stops card being moved if it's locked in machine

            punchCardNode.MovePhysicalCardWithMouseMotion(mouseDragMotion.Y, mouseDragSensitivity);
        }
    }

    protected override void HandleButtonEngaged(int buttonIndex)
    {
        switch (buttonIndex)
        {
            case 0:
                // Execute behavior for button 0
                EnterDigitToMachine(0);
                break;
            case 1:
                // Execute behavior for button 1
                EnterDigitToMachine(1);
                break;
            case 2:
                // Execute behavior for button 2
                EnterDigitToMachine(2);
                break;
            case 3:
                // Execute behavior for button 3
                EnterDigitToMachine(3);
                break;
            case 4:
                // Execute behavior for button 4
                EnterDigitToMachine(4);
                break;
            case 5:
                // Execute behavior for button 5
                EnterDigitToMachine(5);
                break;
            case 6:
                // Execute behavior for button 6
                EnterDigitToMachine(6);
                break;
            case 7:
                // Execute behavior for button 7
                EnterDigitToMachine(7);
                break;
            case 8:
                // Execute behavior for button 8
                EnterDigitToMachine(8);
                break;
            case 9:
                // Execute behavior for button 9
                EnterDigitToMachine(9);
                break;
            default:
                GD.Print("Unhandled button press");
                break;
        }
    }

    // NOT REQUIRED FOR CLOCK IN STATION AS BUTTONS ARE NOT DESIGNED TO STAY PRESSED
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

    private void AssignDebugLabelValues()
    {
        // Assign buttons 0 - 9 to be results of loop through ints
        for (int i = 0; i < buttonsNode.ButtonArray.Length; i++)
        {
            buttonsNode.ButtonArray[i].AssignDebugLabelText($"{i}");
        }
    }

    private void EnterDigitToMachine(int digit)
    {
        if (!cardInMachine) { return; } // Only allows code entry when card is inserted

        codeComponent.EnterDigitToMachine(digit);
    }

    private void HandleCardTargetReached()
    {
        GD.Print("Card target reached");
        cardInMachine = true;
    }

    private void HandleCorrectCodeEntered(bool correct)
    {
        if (!correct) { return; }

        if (!clockedIn)
        {
            clockedIn = true;
            globalSignals.RaisePlayerClockedIn();
            globalSignals.RaisePlayerExitStation(StationType);
        }
        else
        {
            clockedIn = false;
            globalSignals.RaisePlayerClockedOut();
            globalSignals.RaisePlayerExitStation(StationType);
        }
    }
}
