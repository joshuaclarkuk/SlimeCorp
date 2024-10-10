using Godot;
using System;
using System.Collections.Generic;

public partial class GlobalSignals : Node
{
    public event Action OnBlackScreenDisappeared;

    public event Action<int[]> OnGenerateEmployeeNumber;

    public event Action OnPlayerClockedIn;
    public event Action OnPlayerClockedOut;

    public event Action<Dictionary<E_IngredientList, bool>> OnCreatureFeedRequest;
    public event Action<Dictionary<E_IngredientList, bool>> OnServeCreatureFood;

    public event Action<Dictionary<E_AreasToClean, bool>> OnAreasToCleanRequest;
    public event Action<Dictionary<E_AreasToClean, bool>> OnAreasToCleanCleaned;

    public event Action OnCreaturePlayRequest;
    public event Action OnCreaturePlayedWith;

    public event Action<float> OnSlimeCanisterRemovedFromStation;
    public event Action OnSlimeCanisterAddedToStation;
    public event Action OnSlimeCanisterTakenFromStorage;

    public event Action<bool> OnCreatureFeedRequestSatisfied; // Used to determine whether request should be cleared or not
    public event Action<bool> OnCreatureCleanRequestSatisfied; // Used to determine whether request should be cleared or not
    public event Action<bool> OnCreaturePlayRequestSatisfied; // Used to determine whether request should be cleared or not

    public event Action<ComputerItemResource> OnToDoItemReceived;
    public event Action<ComputerItemResource> OnEmailReceived;
    public event Action OnEmailsRead;
    public event Action<ComputerItemResource> OnNewsArticleReceived;

    public event Action<E_StationType> OnPlayerEnterStationCollider;
    public event Action<E_StationType> OnPlayerExitStationCollider;
    public event Action<E_StationType> OnPlayerInteractWithStation;
    public event Action<E_StationType> OnPlayerExitStation;
    public event Action OnPlayerCanMoveAgain; // Prevents player moving before camera has returned

    public event Action<bool> OnPlayerAtRiskOfFailing;
    public event Action OnPlayerFailureState;
    public event Action OnPlayerWinState;

    public void RaiseOnBlackScreenDisappeared() { OnBlackScreenDisappeared?.Invoke(); }

    public void RaiseGenerateEmployeeNumber(int[] employeeNumber) { OnGenerateEmployeeNumber?.Invoke(employeeNumber); }

    public void RaisePlayerClockedIn() { OnPlayerClockedIn?.Invoke(); }
    public void RaisePlayerClockedOut() { OnPlayerClockedOut?.Invoke(); }

    public void RaiseCreatureFeedRequest(Dictionary<E_IngredientList, bool> requestedIngredients) { OnCreatureFeedRequest?.Invoke(requestedIngredients); }
    public void RaiseServeCreatureFood(Dictionary<E_IngredientList, bool> servedIngredients) { OnServeCreatureFood?.Invoke(servedIngredients); }

    public void RaiseAreasToCleanRequest(Dictionary<E_AreasToClean, bool> areasToClean) { OnAreasToCleanRequest?.Invoke(areasToClean); }
    public void RaiseAreasToCleanCleaned(Dictionary<E_AreasToClean, bool> areasCleaned) { OnAreasToCleanCleaned?.Invoke(areasCleaned); }

    public void RaiseCreaturePlayRequest() { OnCreaturePlayRequest?.Invoke(); }
    public void RaiseCreaturePlayedWith() { OnCreaturePlayedWith?.Invoke(); }

    public void RaiseCreatureFeedRequestSatisfied(bool isSatisfied) { OnCreatureFeedRequestSatisfied?.Invoke(isSatisfied); }
    public void RaiseCreatureCleanRequestSatisfied(bool isSatisfied) { OnCreatureCleanRequestSatisfied?.Invoke(isSatisfied); }
    public void RaiseCreaturePlayRequestSatisfied(bool isSatisfied) { OnCreaturePlayRequestSatisfied?.Invoke(isSatisfied); }

    public void RaiseToDoItemReceived(ComputerItemResource toDoItemResource) { OnToDoItemReceived?.Invoke(toDoItemResource); }
    public void RaiseEmailReceived(ComputerItemResource emailResource) { OnEmailReceived?.Invoke(emailResource); }
    public void RaiseEmailsRead() { OnEmailsRead?.Invoke(); }
    public void RaiseNewsArticleReceived(ComputerItemResource articleResource) { OnNewsArticleReceived?.Invoke(articleResource); }

    public void RaiseSlimeCanisterRemovedFromStation(float slimeAmount) { OnSlimeCanisterRemovedFromStation?.Invoke(slimeAmount); }
    public void RaiseSlimeCanisterAddedToStation() { OnSlimeCanisterAddedToStation?.Invoke(); }
    public void RaiseSlimeCanisterTakenFromStorage() {  OnSlimeCanisterTakenFromStorage?.Invoke(); }

    public void RaisePlayerEnterStationCollider(E_StationType stationType) { OnPlayerEnterStationCollider?.Invoke(stationType); }
    public void RaisePlayerExitStationCollider(E_StationType stationType) { OnPlayerExitStationCollider?.Invoke(stationType); }
    public void RaisePlayerInteractWithStation(E_StationType stationType) { OnPlayerInteractWithStation?.Invoke(stationType); }
    public void RaisePlayerExitStation(E_StationType stationType) { OnPlayerExitStation?.Invoke(stationType); }
    public void RaisePlayerCanMoveAgain() { OnPlayerCanMoveAgain?.Invoke(); }

    public void RaisePlayerAtRiskOfFailing(bool atRisk) { OnPlayerAtRiskOfFailing?.Invoke(atRisk); }
    public void RaisePlayerFailureState() { OnPlayerFailureState?.Invoke(); }
    public void RaisePlayerWinState() { OnPlayerWinState?.Invoke(); }
}
