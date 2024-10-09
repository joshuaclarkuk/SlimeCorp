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

    [ExportCategory("Computer Items")]
    [Export] private ComputerItemResource[] emailItemResources = null;
    [Export] private ComputerItemResource[] newsItemResources = null;

    [ExportCategory("Debug UI")]
    [Export] private DebugUI debugUI = null;

    // Globals
    private GlobalValues globalValues = null;
    private GlobalSignals globalSignals = null;
    private GlobalEvents globalEvents = null;
    private Player player = null;

    // Day counter
    private int currentDayIndex = 0;
    private int maxDays = 10;

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
        globalSignals.OnPlayerClockedOut += HandlePlayerClockedOut; // End day

        // Connect station-based signals
        globalSignals.OnSlimeCanisterRemovedFromStation += HandleSlimeCanisterRemovedFromStation;

        // Send initial stack of emails to computer
        foreach (ComputerItemResource emailResource in emailItemResources)
        {
            globalSignals.RaiseEmailReceived(emailResource);
        }

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
        globalSignals.OnPlayerClockedOut -= HandlePlayerClockedOut;

        // Disconnect station-based signals
        globalSignals.OnSlimeCanisterRemovedFromStation -= HandleSlimeCanisterRemovedFromStation;
    }

    private void HandleBlackScreenDisappeared()
    {
        switch (currentDayIndex)
        {
            case 0:
                // Day 0 logic here
                StartNewDay(currentDayIndex, "Orientation", 3, 100.0f);
                GenerateEmployeeNumber();
                break;
            case 1:
                // Day 1 logic here
                StartNewDay(currentDayIndex, "Day 1", 5, 300.0f);
                break;
            case 2:
                // Day 2 logic here
                StartNewDay(currentDayIndex, "Day 2", 5, 300.0f);
                break;
            case 3:
                // Day 3 logic here
                StartNewDay(currentDayIndex, "Day 3", 5, 300.0f);
                break;
            case 4:
                // Day 4 logic here
                StartNewDay(currentDayIndex, "Day 4", 5, 300.0f);
                break;
            case 5:
                // Day 5 logic here
                StartNewDay(currentDayIndex, "Day 5", 5, 300.0f);
                break;
            case 6:
                // Day 6 logic here
                StartNewDay(currentDayIndex, "Day 6", 5, 300.0f);
                break;
            case 7:
                // Day 7 logic here
                StartNewDay(currentDayIndex, "Final Day", 5, 300.0f);
                break;
            default:
                GD.PrintErr("Have gone past final day, exiting");
                break;
        }
    }

    private void StartNewDay(int dayIndex, string titleToDisplay, int minutesInDay, float slimeCollectionTarget)
    {
        if (dailyNeedResources[dayIndex] != null)
        {
            // Initialise Creature Need depletion rates
            creatureNeeds.SetCreatureNeedDepletionRatesFromResource(
                dailyNeedResources[dayIndex].HungerDepletionRate, 
                dailyNeedResources[dayIndex].HappinessDepletionRate, 
                dailyNeedResources[dayIndex].CleanlinessDepletionRate);
            creatureNeeds.SetMaxNeedReplenishmentRatesFromResource(
                dailyNeedResources[dayIndex].MaxHungerReplenishment,
                dailyNeedResources[dayIndex].MaxCleanlinessReplenishment,
                dailyNeedResources[dayIndex].MaxHappinessReplenishment,
                dailyNeedResources[dayIndex].MaxWasteProductToAdd);

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

    private void HandlePlayerClockedOut()
    {
        // End day logic here
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

    private void AddNewsArticleToComputer(int articleIndex)
    {
        // Add news item to computer
        // Pass in news article resource
    }

    private void AddEmailToComputer(int emailIndex)
    {
        // Add email to computer
        // Pass in email resource
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

    private void EndGame()
    {
        // Initiate final sequence
    }
}
