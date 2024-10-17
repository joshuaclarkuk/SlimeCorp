using Godot;
using System;

public partial class Main : Node3D
{
    [ExportCategory("Player Start")]
    [Export] private Marker3D playerStartNode = null;

    [ExportCategory("Titles")]
    [Export] private IntroCredits introCreditsNode = null;
    [Export] private BlackScreen blackScreenNode = null;
    [Export] private TitleCard titleCardNode = null;

    [ExportCategory("Creature")]
    [Export] private CreatureNeeds creatureNeeds = null;

    [ExportCategory("Creature Need Resources")]
    [Export] private DailyNeedResource[] dailyNeedResources = null;

    [ExportCategory("Stations")]
    [Export] private Stations stationsHeaderNode = null;

    [ExportCategory("Music")]
    [Export] private AudioStreamPlayer nonDiegeticMusicNode = null;

    [ExportCategory("Event Nodes")]
    [Export] private Node3D lightingHeaderNode = null;

    [ExportCategory("Debug UI")]
    [Export] private DebugUI debugUI = null;

    [ExportCategory("Current Day")]
    [Export] private int currentDayIndex = 1;
    [Export] private int maxDays = 5;

    // Globals
    private GlobalValues globalValues = null;
    private GlobalSignals globalSignals = null;
    private GlobalEvents globalEvents = null;
    private Player player = null;

    // Slime counter
    private float requestedSlimeAmountForDay = 0.0f;
    private float slimeCollectedInDay = 0.0f;

    public override void _Ready()
    {
        // Get reference to globals
        globalValues = GetNode<GlobalValues>("/root/GlobalValues");
        globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");
        globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");
        player = GetNode<Player>("/root/Player");

        // Connect timeline signals
        globalSignals.OnBlackScreenDisappeared += HandleBlackScreenDisappeared; // Start new day
        globalSignals.OnPlayerClockedIn += HandlePlayerClockedIn;
        globalSignals.OnShiftIsOver += HandleShiftIsOver;
        globalSignals.OnPlayerClockedOut += HandlePlayerClockedOut; // End day

        // Connect station-based signals
        globalSignals.OnSlimeCanisterRemovedFromStation += HandleSlimeCanisterRemovedFromStation;

        // Connect win/failure states
        globalSignals.OnPlayerFailureState += HandlePlayerFailureState;
        globalSignals.OnPlayerWinState += HandlePlayerWinState;
        globalSignals.OnCreatureWinState += HandleCreatureWinState;

        // Set player to player start
        player.GlobalTransform = playerStartNode.GlobalTransform;

        // Capture mouse
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _ExitTree()
    {
        // Disconnect timeline signals
        globalSignals.OnBlackScreenDisappeared -= HandleBlackScreenDisappeared;
        globalSignals.OnPlayerClockedIn -= HandlePlayerClockedIn;
        globalSignals.OnShiftIsOver -= HandleShiftIsOver;
        globalSignals.OnPlayerClockedOut -= HandlePlayerClockedOut;

        // Disconnect station-based signals
        globalSignals.OnSlimeCanisterRemovedFromStation -= HandleSlimeCanisterRemovedFromStation;

        // Disconnect win/failure states
        globalSignals.OnPlayerFailureState -= HandlePlayerFailureState;
        globalSignals.OnPlayerWinState -= HandlePlayerWinState;
        globalSignals.OnCreatureWinState -= HandleCreatureWinState;
    }

    private void HandleBlackScreenDisappeared()
    {
        switch (currentDayIndex)
        {
            case 0:
                // Day 0 logic here
                StartNewDay(currentDayIndex); // NO LONGER IN USE
                break;
            case 1:
                // Day 1 logic here
                StartNewDay(currentDayIndex);
                GenerateEmployeeNumber();
                break;
            case 2:
                // Day 2 logic here
                StartNewDay(currentDayIndex);
                break;
            case 3:
                // Day 3 logic here
                StartNewDay(currentDayIndex);
                break;
            case 4:
                // Day 4 logic here
                StartNewDay(currentDayIndex);
                break;
            case 5:
                // Day 5 logic here
                StartNewDay(currentDayIndex);
                break;
            case 6:
                // Day 6 logic here
                StartNewDay(currentDayIndex);
                break;
            case 7:
                // Day 7 logic here
                StartNewDay(currentDayIndex);
                break;
            default:
                GD.PrintErr("Have gone past final day, exiting");
                break;
        }
    }

    private void StartNewDay(int dayIndex)
    {
        if (dailyNeedResources[dayIndex] != null)
        {
            // Initialise Creature Need depletion rates and set request timers
            creatureNeeds.SetCreatureNeedDepletionRatesFromResource(
                dailyNeedResources[dayIndex].HungerDepletionRate, 
                dailyNeedResources[dayIndex].HappinessDepletionRate, 
                dailyNeedResources[dayIndex].CleanlinessDepletionRate);
            creatureNeeds.SetMaxNeedReplenishmentRatesFromResource(
                dailyNeedResources[dayIndex].MaxHungerReplenishment,
                dailyNeedResources[dayIndex].MaxCleanlinessReplenishment,
                dailyNeedResources[dayIndex].MaxHappinessReplenishment,
                dailyNeedResources[dayIndex].MaxWasteProductToAdd);
            creatureNeeds.SetRequestTimers();
            creatureNeeds.SetMaxRequests();

            // Load in to do resources
            if (dailyNeedResources[dayIndex].ToDoItemResource != null && dailyNeedResources[dayIndex].ToDoItemResource.Length > 0)
            {
                foreach (ComputerItemResource resource in dailyNeedResources[dayIndex].ToDoItemResource)
                {
                    globalSignals.RaiseToDoItemReceived(resource);
                }
            }
            else
            {
                GD.PrintErr($"No EmailItemResources found for day {dayIndex}");
            }

            // Load in email resources
            if (dailyNeedResources[dayIndex].EmailItemResources != null && dailyNeedResources[dayIndex].EmailItemResources.Length > 0)
            {
                foreach (ComputerItemResource resource in dailyNeedResources[dayIndex].EmailItemResources)
                {
                    globalSignals.RaiseEmailReceived(resource);
                }
            }
            else
            {
                GD.PrintErr($"No EmailItemResources found for day {dayIndex}");
            }

            // Load in news item resources
            if (dailyNeedResources[dayIndex].NewsItemResources != null && dailyNeedResources[dayIndex].NewsItemResources.Length > 0)
            {
                foreach (ComputerItemResource resource in dailyNeedResources[dayIndex].NewsItemResources)
                {
                    globalSignals.RaiseNewsArticleReceived(resource);
                }
            }
            else
            {
                GD.PrintErr($"No NewsItemResources found for day {dayIndex}");
            }

            // Set up Title and outline daily parameters
            titleCardNode.UpdateTextAndDisplay(dailyNeedResources[dayIndex].TitleToDisplay);
            creatureNeeds.SetUpForNewDay(dailyNeedResources[dayIndex].MinutesInDay);
            ResetSlimeCollectedAndSetNewTarget(dailyNeedResources[dayIndex].SlimeCollectionTarget);
        }
        else
        {
            GD.PrintErr("No daily resource found for this day");
        }

        // Initialise player camera and movement
        player.MakeCameraCurrent();
        stationsHeaderNode.isTransitioningBetweenDays = false;
    }

    private void HandlePlayerClockedIn()
    {
        // Play intro credits scrawl if it's day zero
        if (currentDayIndex == 0)
        {
            introCreditsNode.PlayIntroCreditsAnimation();
        }

        // Start music
        nonDiegeticMusicNode.Play();
    }

    private void HandleShiftIsOver()
    {
        GD.Print("Shift is over. Player needs to clock out");
        // Lock stations
        // Let timers run, need to make sure oil in canister ISN'T banked

    }

    private void HandlePlayerClockedOut()
    {
        if (globalValues.HasPlayerInjectedCreature)
        {
            globalSignals.RaisePlayerWinState();
            return;
        }

        if (currentDayIndex == maxDays && !globalValues.HasPlayerInjectedCreature)
        {
            globalSignals.RaiseCreatureWinState();
        }

        // End day logic here
        if (HasPlayerBankedEnoughSlime())
        {
            ProgressToNextDay();
        }
        else
        {
            globalSignals.RaisePlayerFailureState();
        }
    }

    private void ProgressToNextDay()
    {
        blackScreenNode.DisplayBlackScreen();
        stationsHeaderNode.isTransitioningBetweenDays = true;
        currentDayIndex++;
        player.GlobalTransform = playerStartNode.GlobalTransform;
        blackScreenNode.WaitSecondsAndDisappear(3.0f);
    }

    private void GenerateEmployeeNumber()
    {
        // Generate employee number here
        Random random = new Random();
        int[] employeeNumber = new int[4];
        for (int i = 0; i < employeeNumber.Length; i++)
        {
            employeeNumber[i] = random.Next(0, 10);
        }
        globalSignals.RaiseGenerateEmployeeNumber(employeeNumber);

        GD.Print($"Generated employee number: {string.Join("", employeeNumber)} vs Official employee number: {string.Join("", globalValues.EmployeeNumber)}");
    }

    private void ResetSlimeCollectedAndSetNewTarget(float newTarget)
    {
        requestedSlimeAmountForDay = newTarget;
        slimeCollectedInDay = 0.0f;
        debugUI.UpdateCurrentSlimeProgressBar(0.0f);
        debugUI.UpdateTotalSlimeCollectedProgressBar(0.0f, requestedSlimeAmountForDay);
    }

    private void HandleSlimeCanisterRemovedFromStation(float slimeAmount)
    {
        slimeCollectedInDay += slimeAmount;
        debugUI.UpdateTotalSlimeCollectedProgressBar(slimeCollectedInDay, requestedSlimeAmountForDay);
        GD.Print($"MAIN: Banked {slimeAmount} slime. Daily total: {slimeCollectedInDay}");
    }

    private bool HasPlayerBankedEnoughSlime()
    {
        if (slimeCollectedInDay >= requestedSlimeAmountForDay)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void HandlePlayerFailureState()
    {
        GD.PrintErr("Player did not meet creatures needs. Got fired.");
        globalValues.SetEndState(E_EndState.FIRED);
        GetTree().ChangeSceneToFile("res://Scenes/Levels/EndScreen.tscn");
    }

    private void HandlePlayerWinState()
    {
        GD.PrintErr("Player destroyed facility. Win!");
        globalValues.SetEndState(E_EndState.PLAYER_WIN);
        GetTree().ChangeSceneToFile("res://Scenes/Levels/EndScreen.tscn");
    }

    private void HandleCreatureWinState()
    {
        GD.PrintErr("Creature destroyed Earth. Lose!");
        globalValues.SetEndState(E_EndState.CREATURE_WIN);
        GetTree().ChangeSceneToFile("res://Scenes/Levels/EndScreen.tscn");
    }
}
