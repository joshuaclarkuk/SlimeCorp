using Godot;
using System;

public partial class ElevatorPanelStation : Station
{
    [ExportCategory("Required Nodes")]
    [Export] private CodeComponent codeComponentNode = null;
    [Export] private ElevatorDoors elevatorDoorsNode = null;
    [Export] private Label3D employeeNumberLabelNode = null;

    private bool playerHasEmployeeCard = false;
    private bool doorsAreOpen = false;

    public override void _Ready()
    {
        base._Ready();

        globalSignals.OnGenerateEmployeeNumber += HandleGenerateEmployeeNumber;
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        globalSignals.OnGenerateEmployeeNumber -= HandleGenerateEmployeeNumber;
    }

    public override void EnterStation()
    {
        base.EnterStation();

        // Link signals
        codeComponentNode.OnCorrectCodeEntered += HandleCorrectCodeEntered;

        GD.Print($"Calling EnterStation method on {Name}");
    }

    public override void ExitStation()
    {
        base.ExitStation();

        // Unlink signals
        codeComponentNode.OnCorrectCodeEntered -= HandleCorrectCodeEntered;

        GD.Print($"Calling ExitStation method on {Name}");
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

    protected override void HandleButtonDisengaged(int buttonIndex)
    {
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

    private void HandleGenerateEmployeeNumber(int[] employeeNumber)
    {
        employeeNumberLabelNode.Text = $"Employee Number\n{string.Join("", globalValues.EmployeeNumber)}";
        playerHasEmployeeCard = true;
    }

    private void EnterDigitToMachine(int digit)
    {
        if (!playerHasEmployeeCard) { return; }

        codeComponentNode.EnterDigitToMachine(digit);
    }

    private void HandleCorrectCodeEntered(bool correct)
    {
        if (!doorsAreOpen)
        {
            elevatorDoorsNode.ToggleElevatorDoorsOpen(true);
            doorsAreOpen = true;
            globalSignals.RaisePlayerExitStation(StationType);
        }
    }
}