using Godot;
using System;

public partial class GlobalValues : Node
{
    public bool HasEmployeeCard { get; private set; } = false;
    public bool HasSupervisorCard { get; private set; } = false;
    public bool HasPoisonInjector { get; private set; } = false;
    public bool HasPlayerInjectedCreature { get; private set; } = false;

    public int[] EmployeeNumber { get; private set; } = new int[] { 0, 0, 0, 0 };

    private GlobalSignals globalSignals = null;

    public override void _Ready()
    {
        globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");
        SubscribeToEvents();
    }

    public override void _ExitTree()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        globalSignals.OnGenerateEmployeeNumber += HandleGenerateEmployeeNumber;
    }

    private void UnsubscribeFromEvents()
    {
        globalSignals.OnGenerateEmployeeNumber -= HandleGenerateEmployeeNumber;
    }

    public void SetHasEmployeeCard(bool hasCard)
    {
        HasEmployeeCard = hasCard;
        GD.Print($"Has employee card: {HasEmployeeCard}");
    }

    public void SetHasSupervisorCard(bool hasSupervisorCard)
    {
        HasSupervisorCard = hasSupervisorCard;
        GD.Print($"Has supervisor card: {HasSupervisorCard}");
    }

    public void SetHasPoisonInjector(bool hasPoisonInjector)
    {
        HasPoisonInjector = hasPoisonInjector;
        GD.Print($"Has poison injector: {HasPoisonInjector}");
    }

    public void SetHasPlayerInjectedCreature(bool hasPlayerInjectedCreature)
    {
        HasPlayerInjectedCreature = hasPlayerInjectedCreature;
    }

    public void SetEmployeeNumber(int[] employeeNumber)
    {
        EmployeeNumber = employeeNumber;
    }

    private void HandleGenerateEmployeeNumber(int[] employeeNumber)
    {
        EmployeeNumber = employeeNumber;
    }
}