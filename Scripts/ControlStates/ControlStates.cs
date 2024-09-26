using Godot;
using System;

public partial class ControlStates : Node3D
{
    private GlobalSignals globalSignals = null;

    public override void _Ready()
    {
        globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");
        globalSignals.OnPlayerInteractWithStation += HandlePlayerInteractWithStation;
        globalSignals.OnPlayerExitStation += HandlePlayerExitStation;

        // Deactivate all control states on launch
        foreach (ControlState controlState in GetChildren())
        {
            DeactivateControlState(controlState);
        }
    }

    public override void _ExitTree()
    {
        globalSignals.OnPlayerInteractWithStation -= HandlePlayerInteractWithStation;
        globalSignals.OnPlayerExitStation -= HandlePlayerExitStation;
    }

    private void HandlePlayerInteractWithStation(E_StationType stationType)
    {
        if (stationType == E_StationType.NONE)
        {
            foreach (ControlState controlState in GetChildren())
            {
                DeactivateControlState(controlState);
            }
        }
        else
        {
            foreach (ControlState controlState in GetChildren())
            {
                if (controlState.stationType == stationType)
                {
                    ActivateControlState(controlState);
                }
                else
                {
                    DeactivateControlState(controlState);
                }
            }
        }
    }

    private void HandlePlayerExitStation(E_StationType type)
    {
        foreach (ControlState controlState in GetChildren())
        {
            DeactivateControlState(controlState);
        }
    }

    private void ActivateControlState(ControlState controlState)
    {
        controlState.SetProcessInput(true);
        controlState.SetProcessUnhandledInput(true);
    }

    private void DeactivateControlState(ControlState controlState)
    {
        controlState.SetProcessInput(false);
        controlState.SetProcessUnhandledInput(false);
    }
}
