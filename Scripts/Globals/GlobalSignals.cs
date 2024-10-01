using Godot;
using System;

public partial class GlobalSignals : Node
{
    public event Action<int> OnStartNewDay;
    public event Action OnEndDay;

    public event Action<int[]> OnEmployeeNumberGenerated;

    public event Action OnPlayerClockedIn;
    public event Action OnPlayerClockedOut;

    public event Action<E_StationType> OnPlayerEnterStationCollider;
    public event Action<E_StationType> OnPlayerExitStationCollider;
    public event Action<E_StationType> OnPlayerInteractWithStation;
    public event Action<E_StationType> OnPlayerExitStation;
    public event Action OnPlayerCanMoveAgain; // Prevents player moving before camera has returned

    public void RaiseStartNewDay(int day) { OnStartNewDay?.Invoke(day); }
    public void RaiseEndDay() { OnEndDay?.Invoke(); }

    public void RaiseEmployeeNumberGenerated(int[] employeeNumber) { OnEmployeeNumberGenerated?.Invoke(employeeNumber); }

    public void RaisePlayerClockedIn() { OnPlayerClockedIn?.Invoke(); }
    public void RaisePlayerClockedOut() { OnPlayerClockedOut?.Invoke(); }

    public void RaisePlayerEnterStationCollider(E_StationType stationType) { OnPlayerEnterStationCollider?.Invoke(stationType); }
    public void RaisePlayerExitStationCollider(E_StationType stationType) { OnPlayerExitStationCollider?.Invoke(stationType); }
    public void RaisePlayerInteractWithStation(E_StationType stationType) { OnPlayerInteractWithStation?.Invoke(stationType); }
    public void RaisePlayerExitStation(E_StationType stationType) { OnPlayerExitStation?.Invoke(stationType); }
    public void RaisePlayerCanMoveAgain() { OnPlayerCanMoveAgain?.Invoke(); }
}
