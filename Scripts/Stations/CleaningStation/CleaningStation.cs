using Godot;
using System;
using System.Collections.Generic;

public partial class CleaningStation : Station
{
    [ExportCategory("Required Nodes")]
    [Export] private Lever leverNode = null;
    [Export] private Timer flushingSystemTimerNode = null;

    private Dictionary<E_AreasToClean, bool> areasToCleanDictionary = new Dictionary<E_AreasToClean, bool>();

    public override void _Ready()
    {
        base._Ready();

        // Store areas to clean enum values
        Array enumValues = Enum.GetValues(typeof(E_AreasToClean));

        // Initialise areas to clean dictionary with all possible areas and set initial values to false
        InitialiseAreasToCleanDictionary(enumValues);

        // Assign button debug label values (leaves 0 clear for "Reset")
        AssignDebugLabelValues(enumValues);

        // Have to link in _Ready and _Exit otherwise would lose track of timer when player exits Station
        flushingSystemTimerNode.Timeout += HandleFlushingSystemTimerTimeout;
    }

    public override void _ExitTree()
    {
        flushingSystemTimerNode.Timeout -= HandleFlushingSystemTimerTimeout;
    }

    public override void EnterStation()
    {
        base.EnterStation();

        // Link signals
        leverNode.OnLeverTargetReached += HandleLeverTargetReached;

        GD.Print($"Calling EnterStation method on {Name}");
    }

    public override void ExitStation()
    {
        base.ExitStation();

        // Unlink signals
        leverNode.OnLeverTargetReached -= HandleLeverTargetReached;

        // Reset machine
        ResetMachine();

        GD.Print($"Calling ExitStation method on {Name}");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

        if (isMouseClicked)
        {
            if (canInteractWithStation)
            {
                leverNode.MovePhysicalHandleWithMouseMotion(mouseDragSensitivity, mouseDragMotion);
            }
        }
        else
        {
            leverNode.ReturnToOriginalPosition();
        }
    }

    public void ResetMachine()
    {
        buttonsNode.ResetAndRaiseAllButtons();
        leverNode.ReturnToOriginalPosition();
    }

    protected override void HandleButtonEngaged(int buttonIndex)
    {
        switch (buttonIndex)
        {
            case 0:
                // Execute behavior for button 0
                buttonsNode.ResetAndRaiseAllButtons();
                break;
            case 1:
                // Execute behavior for button 1
                AddAreaToCleanToList(0);
                break;
            case 2:
                // Execute behavior for button 2
                AddAreaToCleanToList(1);
                break;
            case 3:
                // Execute behavior for button 3
                AddAreaToCleanToList(2);
                break;
            case 4:
                // Execute behavior for button 4
                AddAreaToCleanToList(3);
                break;
            case 5:
                // Execute behavior for button 5
                AddAreaToCleanToList(4);
                break;
            case 6:
                // Execute behavior for button 6
                AddAreaToCleanToList(5);
                break;
            case 7:
                // Execute behavior for button 7
                AddAreaToCleanToList(6);
                break;
            case 8:
                // Execute behavior for button 8
                AddAreaToCleanToList(7);
                break;
            case 9:
                // Execute behavior for button 9
                AddAreaToCleanToList(8);
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
                RemoveAreaToCleanFromList(0);
                break;
            case 2:
                // Execute behavior for button 2
                RemoveAreaToCleanFromList(1);
                break;
            case 3:
                // Execute behavior for button 3
                RemoveAreaToCleanFromList(2);
                break;
            case 4:
                // Execute behavior for button 4
                RemoveAreaToCleanFromList(3);
                break;
            case 5:
                // Execute behavior for button 5
                RemoveAreaToCleanFromList(4);
                break;
            case 6:
                // Execute behavior for button 6
                RemoveAreaToCleanFromList(5);
                break;
            case 7:
                // Execute behavior for button 7
                RemoveAreaToCleanFromList(6);
                break;
            case 8:
                // Execute behavior for button 8
                RemoveAreaToCleanFromList(7);
                break;
            case 9:
                // Execute behavior for button 9
                RemoveAreaToCleanFromList(8);
                break;
            default:
                GD.Print("Unhandled button press");
                break;
        }
    }

    private void InitialiseAreasToCleanDictionary(Array enumValues)
    {
        for (int i = 0; i < enumValues.Length; i++)
        {
            areasToCleanDictionary.Add((E_AreasToClean)enumValues.GetValue(i), false);
        }
    }

    private void AssignDebugLabelValues(Array enumValues)
    {
        // Set button 0 to be "Reset"
        buttonsNode.ButtonArray[0].AssignDebugLabelText(" 0\nReset");

        // Assign buttons 1 - 9 to be results of loop through enum
        int enumIndex = 0;
        for (int j = 1; j < buttonsNode.ButtonArray.Length; j++)
        {
            if (enumIndex < enumValues.Length)
            {
                buttonsNode.ButtonArray[j].AssignDebugLabelText($"{j}\n{enumValues.GetValue(enumIndex)}");
                enumIndex++;
            }
        }
    }

    private void AddAreaToCleanToList(int areaIndex)
    {
        E_AreasToClean areaEnum = (E_AreasToClean)areaIndex;
        areasToCleanDictionary[areaEnum] = true;
    }

    private void RemoveAreaToCleanFromList(int areaIndex)
    {
        E_AreasToClean areaEnum = (E_AreasToClean)areaIndex;
        areasToCleanDictionary[areaEnum] = false;
    }

    private void HandleLeverTargetReached()
    {
        GD.Print("Handling lever target reached on cleaning station");
        flushingSystemTimerNode.Start();
        canInteractWithStation = false;
        globalSignals.RaiseAreasToCleanCleaned(areasToCleanDictionary);
        globalSignals.RaisePlayerExitStation(StationType);

        // Debug
        foreach (E_AreasToClean area in areasToCleanDictionary.Keys)
        {
            GD.Print($"Area cleaned: {area} | {areasToCleanDictionary[area]}");
        }
    }

    private void HandleFlushingSystemTimerTimeout()
    {
        canInteractWithStation = true;
    }
}
