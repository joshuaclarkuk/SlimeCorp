using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class CleaningComponent : Node3D
{
    public List<E_AreasToClean> RequestedAreasList { get; private set; } = new List<E_AreasToClean>();

    private GlobalSignals globalSignals = null;

    // Feeding requests
    private Array areaEnumValues;

    public event Action<bool> OnAreaCleaned;

    public override void _Ready()
    {
        // Link signals
        globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");
        globalSignals.OnAreaCleaned += HandleAreaCleaned;

        // Initialise possible areas array and compile dictionary
        areaEnumValues = Enum.GetValues(typeof(E_AreasToClean));

        // Clear request list to be safe
        RequestedAreasList.Clear();
    }

    private void HandleAreaCleaned(E_AreasToClean clean)
    {
        throw new NotImplementedException();
    }

    public override void _ExitTree()
    {
        globalSignals.OnAreaCleaned -= HandleAreaCleaned;
    }

    public void ProcessCleaningRequest()
    {
        GD.Print("Processing cleaning request");
        int randomAreaIndex = GD.RandRange(0, areaEnumValues.Length - 1);
        E_AreasToClean randomArea = (E_AreasToClean)areaEnumValues.GetValue(randomAreaIndex);
        RequestedAreasList.Add(randomArea);
        globalSignals.RaiseAreaToCleanRequest(RequestedAreasList);
    }

    private void HandleAreaCleaned(List<E_AreasToClean> areaCleaned)
    {
        foreach (E_AreasToClean area in areaCleaned)
        {
            if (RequestedAreasList.Contains(area))
            {
                OnAreaCleaned?.Invoke(true);
                RequestedAreasList.Remove(area);
            }
            else
            {
                OnAreaCleaned?.Invoke(false);
            }
        }
    }
}
