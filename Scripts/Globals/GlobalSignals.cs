using Godot;
using System;

public partial class GlobalSignals : Node
{
    public event Action<int> OnStartNewDay;
    public event Action OnEndDay;

    public event Action OnPlayerClockedIn;
    public event Action OnPlayerClockedOut;

    public event Action<E_StationType> OnPlayerEnterStationCollider;
    public event Action<E_StationType> OnPlayerExitStationCollider;
    public event Action<E_StationType> OnPlayerInteractWithStation;

    public void RaiseStartNewDay(int day) { OnStartNewDay?.Invoke(day); }
    public void RaiseEndDay() { OnEndDay?.Invoke(); }

    public void RaisePlayerClockedIn() { OnPlayerClockedIn?.Invoke(); }
    public void RaisePlayerClockedOut() { OnPlayerClockedOut?.Invoke(); }

    public void RaisePlayerEnterStationCollider(E_StationType stationType) { OnPlayerEnterStationCollider?.Invoke(stationType); }
    public void RaisePlayerExitStationCollider(E_StationType stationType) { OnPlayerExitStationCollider?.Invoke(stationType); }
    public void RaisePlayerInteractWithStation(E_StationType stationType) { OnPlayerInteractWithStation?.Invoke(stationType); }
}
