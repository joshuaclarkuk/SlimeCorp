using Godot;
using System;
using System.Collections.Generic;

public partial class AreaCleanRequestBoard : Node3D
{
    [ExportCategory("Required Nodes")]
    [Export] private AreaButton[] areaButtons = new AreaButton[9];

    private E_AreasToClean[] areaEnumValues = null;
    private GlobalSignals globalSignals = null;
    private List<E_AreasToClean> activeAreasToClean = new List<E_AreasToClean>();

    public override void _Ready()
    {
        globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");
        globalSignals.OnAreasToCleanRequest += HandleAreasToCleanRequest;
        globalSignals.OnAreaCleaned += HandleAreasCleaned;

        // Initialise array
        areaEnumValues = (E_AreasToClean[])Enum.GetValues(typeof(E_AreasToClean));

        // Assign buttons 1 - 9 to be results of loop through enum
        int enumIndex = 0;
        for (int i = 0; i < areaButtons.Length; i++)
        {
            if (enumIndex < areaEnumValues.Length)
            {
                areaButtons[i].AssignEnumValue(areaEnumValues[enumIndex]);
                enumIndex++;
            }
        }
    }

    public override void _ExitTree()
    {
        globalSignals.OnAreasToCleanRequest -= HandleAreasToCleanRequest;
        globalSignals.OnAreaCleaned -= HandleAreasCleaned;
    }

    private void HandleAreasToCleanRequest(List<E_AreasToClean> list)
    {
        // Clear list and deactivate buttons
        activeAreasToClean.Clear();
        foreach (AreaButton button in areaButtons)
        {
            button.DeactivateButton();
        }

        // Build new list and activate relevant buttons
        foreach (E_AreasToClean area in list)
        {
            activeAreasToClean.Add(area);

            foreach (AreaButton button in areaButtons)
            {
                if (button.AssignedArea == area)
                {
                    button.ActivateButton();
                }
            }
        }
    }

    private void HandleAreasCleaned(List<E_AreasToClean> list)
    {
        // Remove cleaned area from list
        foreach (E_AreasToClean area in list)
        {
            if (activeAreasToClean.Contains(area))
            {
                activeAreasToClean.Remove(area);
            }
        }

        // Deactivate all buttons and activate relevant buttons based on remaining areas
        foreach (AreaButton button in areaButtons)
        {
            button.DeactivateButton();
        }
        foreach (E_AreasToClean area in activeAreasToClean)
        {
            foreach (AreaButton button in areaButtons)
            {
                if (button.AssignedArea == area)
                {
                    button.ActivateButton();
                }
            }
        }
    }
}
