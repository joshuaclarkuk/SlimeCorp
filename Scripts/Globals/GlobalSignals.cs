using Godot;
using System;
using System.Collections.Generic;

public partial class GlobalSignals : Node
{
    public event Action<int> OnStartNewDay;
    public event Action OnEndDay;

    public event Action<int[]> OnEmployeeNumberGenerated;

    public event Action OnPlayerClockedIn;
    public event Action OnPlayerClockedOut;

    public event Action<Dictionary<E_IngredientList, bool>> OnCreatureFeedRequest;
    public event Action<Dictionary<E_IngredientList, bool>> OnServeCreatureFood;

    public event Action<Dictionary<E_AreasToClean, bool>> OnAreasToCleanRequest;
    public event Action<Dictionary<E_AreasToClean, bool>> OnAreasToCleanCleaned;

    public event Action<bool> OnCreatureFeedRequestSatisfied; // Used to determine whether request should be cleared or not
    public event Action<bool> OnCreatureCleanRequestSatisfied; // Used to determine whether request should be cleared or not

    public event Action<bool> OnSlimeBarrelIsAttached; // Used to determine whether slime gauge can fill or not

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

    public void RaiseCreatureFeedRequest(Dictionary<E_IngredientList, bool> requestedIngredients) { OnCreatureFeedRequest?.Invoke(requestedIngredients); }
    public void RaiseServeCreatureFood(Dictionary<E_IngredientList, bool> servedIngredients) { OnServeCreatureFood?.Invoke(servedIngredients); }

    public void RaiseCreatureFeedRequestSatisfied(bool isSatisfied) { OnCreatureFeedRequestSatisfied?.Invoke(isSatisfied); }
    public void RaiseCreatureCleanRequestSatisfied(bool isSatisfied) { OnCreatureCleanRequestSatisfied?.Invoke(isSatisfied); }

    public void RaiseAreasToCleanRequest(Dictionary<E_AreasToClean, bool> areasToClean) { OnAreasToCleanRequest?.Invoke(areasToClean); }
    public void RaiseAreasToCleanCleaned(Dictionary<E_AreasToClean, bool> areasCleaned) { OnAreasToCleanCleaned?.Invoke(areasCleaned); }

    public void RaisePlayerEnterStationCollider(E_StationType stationType) { OnPlayerEnterStationCollider?.Invoke(stationType); }
    public void RaisePlayerExitStationCollider(E_StationType stationType) { OnPlayerExitStationCollider?.Invoke(stationType); }
    public void RaisePlayerInteractWithStation(E_StationType stationType) { OnPlayerInteractWithStation?.Invoke(stationType); }
    public void RaisePlayerExitStation(E_StationType stationType) { OnPlayerExitStation?.Invoke(stationType); }
    public void RaisePlayerCanMoveAgain() { OnPlayerCanMoveAgain?.Invoke(); }
}
