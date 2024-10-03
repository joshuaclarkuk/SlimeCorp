using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class CleaningComponent : Node3D
{
    [ExportCategory("Behaviour")]
    [Export] private int maxAreasRequested = Enum.GetValues(typeof(E_AreasToClean)).Length;

    private GlobalSignals globalSignals = null;

    // Feeding requests
    private Array areaEnumValues;
    private Dictionary<E_AreasToClean, bool> requestedAreasDictionary = new Dictionary<E_AreasToClean, bool>();
    private int numAreasRequested = 0;

    public event Action<E_NeedMetAmount> OnAreaCleaned;

    public override void _Ready()
    {
        // Link signals
        globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");
        globalSignals.OnAreasToCleanCleaned += HandleAreasToCleanCleaned;

        // Initialise possible areas array and compile dictionary
        areaEnumValues = Enum.GetValues(typeof(E_AreasToClean));

        // Initialise requestedAreasDictionary with all possible keys and set values to false
        foreach (E_AreasToClean area in Enum.GetValues(typeof(E_AreasToClean)))
        {
            requestedAreasDictionary[area] = false;
        }
    }

    public override void _ExitTree()
    {
        globalSignals.OnAreasToCleanCleaned -= HandleAreasToCleanCleaned;
    }

    public void ProcessCleaningRequest()
    {
        // Make sure all values are set to false before making a new request for safety
        foreach (E_AreasToClean area in requestedAreasDictionary.Keys.ToList())
        {
            requestedAreasDictionary[area] = false;
        }

        numAreasRequested = 0;

        // Hoping this makes area selection more uniform rather than just running top to bottom
        List<E_AreasToClean> shuffledAreas = areaEnumValues.Cast<E_AreasToClean>().OrderBy(x => GD.Randf()).ToList();

        // Loop through possible areas and decide whether or not it is wanted
        foreach (E_AreasToClean area in shuffledAreas)
        {
            // Break early if the max number of areas have already been requested
            if (numAreasRequested >= maxAreasRequested)
            {
                break;
            }

            // Randomly decide if the area is dirty (50% chance)
            bool isDirty = GD.Randf() < 0.5f;

            // If the area is dirty, set it to true in the dictionary
            if (isDirty)
            {
                requestedAreasDictionary[area] = true;
                numAreasRequested++;
            }
        }

        // If no areas were selected, make another request
        if (numAreasRequested == 0)
        {
            GD.Print("No ingredients requested, will retry.");
            ProcessCleaningRequest();
        }
        else
        {
            globalSignals.RaiseAreasToCleanRequest(requestedAreasDictionary);

            //// Debug request
            //foreach (E_AreasToClean area in requestedAreasDictionary.Keys)
            //{
            //    GD.Print($"Area to clean: {area} is dirty: {requestedAreasDictionary[area]}");
            //}
        }
    }

    private void HandleAreasToCleanCleaned(Dictionary<E_AreasToClean, bool> areaCleaned)
    {
        int numberOfRequestedAreas = requestedAreasDictionary.Values.Count(value => value == true);
        int numberOfCorrectAreasCleaned = 0;

        foreach (E_AreasToClean area in Enum.GetValues(typeof(E_AreasToClean)))
        {
            bool requestedValue = requestedAreasDictionary.ContainsKey(area) ? requestedAreasDictionary[area] : false;
            bool cleanedValue = areaCleaned.ContainsKey(area) ? areaCleaned[area] : false;

            // Compare the requested and cleaned values for the current ingredient
            if (requestedValue && cleanedValue)
            {
                numberOfCorrectAreasCleaned++;
            }
        }

        // Calculate percentage of correct areas based on number requested
        float percentageCorrectAreas = (float)numberOfCorrectAreasCleaned / numberOfRequestedAreas * 100;
        GD.Print($"Percentage correct: {percentageCorrectAreas}");

        // Switch statement based on percentage of correct ingredients
        switch (percentageCorrectAreas)
        {
            case <= 50.0f:
                GD.Print("Less than or equal to 50% of areas cleaned correctly. Apply penalty.");
                OnAreaCleaned?.Invoke(E_NeedMetAmount.NONE);
                break;

            case > 50.0f and <= 75.0f:
                GD.Print("Between 51% and 75% of areas cleaned correctly. Neutral outcome.");
                OnAreaCleaned?.Invoke(E_NeedMetAmount.HALF);
                break;

            case > 75.0f and < 100.0f:
                GD.Print("Between 76% and 99% of areas cleaned correctly. Good outcome.");
                OnAreaCleaned?.Invoke(E_NeedMetAmount.MOST);
                break;

            case 100.0f:
                GD.Print("100% of areas cleaned correctly. Perfect score.");
                OnAreaCleaned?.Invoke(E_NeedMetAmount.ALL);
                break;
        }
    }
}
