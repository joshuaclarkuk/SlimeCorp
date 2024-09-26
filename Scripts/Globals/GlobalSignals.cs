using Godot;
using System;

public partial class GlobalSignals : Node
{
    public event Action<int> OnStartNewDay;
    public event Action OnEndDay;

    public event Action OnPlayerClockedIn;
    public event Action OnPlayerClockedOut;

    public void RaiseStartNewDay(int day) { OnStartNewDay?.Invoke(day); }
    public void RaiseEndDay() { OnEndDay?.Invoke(); }
    public void RaisePlayerClockedIn() { OnPlayerClockedIn?.Invoke(); }
    public void RaisePlayerClockedOut() { OnPlayerClockedOut?.Invoke(); }
}
