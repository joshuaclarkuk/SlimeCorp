using Godot;
using System;

public partial class GlobalValues : Node
{
    public bool HasEmployeeCard { get; private set; } = false;

    public int[] EmployeeNumber { get; private set; } = new int[] { 0, 0, 0, 0 };

    private GlobalSignals globalSignals = null;

    public override void _Ready()
    {
        globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");
        
    }

    public void SetHasEmployeeCard(bool hasCard)
    {
        HasEmployeeCard = hasCard;
        GD.Print($"Has employee card: {HasEmployeeCard}");
    }

    public void GenerateEmployeeNumber()
    {
        // Generate employee number here
        Random random = new Random();
        int[] employeeNumber = new int[4];
        for (int i = 0; i < employeeNumber.Length; i++)
        {
            employeeNumber[i] = random.Next(0, 10);
        }
        EmployeeNumber = employeeNumber;

        GD.Print($"Generated employee number: {string.Join("",employeeNumber)} vs Official employee number: {string.Join("", EmployeeNumber)}");
    }
}