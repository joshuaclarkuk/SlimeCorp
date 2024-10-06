using Godot;
using System;

public partial class EmployeeCardPickup : Interactable
{
    Node3D employeeCardEvent = null;

    public override void _EnterTree()
    {
        // Get reference to GlobalValues autoload to update persistent bool
        if (globalValues == null)
        {
            globalValues = GetNode<GlobalValues>("/root/GlobalValues");
        }
    }

    public override void Interact()
    {
        globalValues.SetHasEmployeeCard(true);
        QueueFree();
    }
}
