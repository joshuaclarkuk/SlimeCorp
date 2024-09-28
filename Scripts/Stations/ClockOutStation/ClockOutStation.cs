﻿using Godot;
using System;

public partial class ClockOutStation : Station
{
    private bool isClockedIn = false;

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
        if (Input.IsActionJustPressed(GlobalConstants.INPUT_MOUSE_1))
        {
            if (!isClockedIn)
            {
                globalSignals.RaisePlayerClockedIn();
                isClockedIn = true;
                GD.Print("Player clocked in");
            }
            else
            {
                globalSignals.RaisePlayerClockedOut();
                isClockedIn = false;
                GD.Print("Player clocked out");
            }
        }
    }
}
