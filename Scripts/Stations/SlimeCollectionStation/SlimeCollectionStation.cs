using Godot;
using System;

public partial class SlimeCollectionStation : Station
{
    [ExportCategory("Required Nodes")]
    [Export] private Valve valveNode = null;
    [Export] private Node3D canisterMeshToAppear = null;
    [Export] private StationNeedsProgressBarComponent stationNeedsProgressBarComponentNode = null;
    [Export] private StationAlertComponent stationAlertComponentNode = null;

    [ExportCategory("AudioNodes")]
    [Export] private AudioStreamPlayer3D slimeCollectionAudioNode = null;
    [Export] private AudioStreamPlayer3D barrelRemovedAudioNode = null;

    [ExportCategory("External Nodes")]
    [Export] private DebugUI debugUI = null;

    [ExportCategory("Slime Collection")]
    [Export] private float baseSlimeCollectionRate = 0.4f;
    [Export] private float maxSlimeInCanister = 100.0f;
    [Export] private float feedingSlimeMultiplier = 2.0f;
    [Export] private float cleaningSlimeMultiplier = 2.0f;
    [Export] private float happinessSlimeMultiplier = 2.0f;

    private float currentSlimeLevel = 0.0f;

    private bool valveIsOpen = false;
    private bool playerIsHoldingBarrel = false;
    private bool canisterInSlot = true;
    private bool hasStartedAudio = false; // Switches harvesting audio loop on and off

    public override void _Ready()
    {
        base._Ready();

        globalSignals.OnSlimeCanisterTakenFromStorage += HandleSlimeCanisterTakenFromStorage;
        globalSignals.OnPlayerClockedOut += HandlePlayerClockedOut;
        valveNode.OnValveTargetReached += HandleValveTargetReached;

        canisterMeshToAppear.Visible = true;
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        globalSignals.OnSlimeCanisterTakenFromStorage -= HandleSlimeCanisterTakenFromStorage;
        globalSignals.OnPlayerClockedOut -= HandlePlayerClockedOut;
        valveNode.OnValveTargetReached -= HandleValveTargetReached;
    }

    public override void EnterStation()
    {
        base.EnterStation();


        TryAddingBarrelToStation();
    }

    public override void ExitStation()
    {
        base.ExitStation();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

        if (isMouseClicked)
        {
            valveNode.MovePhysicalValveWithMouseMotion(mouseDragSensitivity, mouseDragMotion);
        }
    }

    protected override void HandleButtonEngaged(int buttonIndex)
    {
        GD.Print($"Button {buttonIndex} was pressed in {Name}.");

        switch (buttonIndex)
        {
            case 0:
                // Execute behavior for button 0
                break;
            case 1:
                // Execute behavior for button 1
                break;
            case 2:
                // Execute behavior for button 2
                break;
            case 3:
                // Execute behavior for button 3
                break;
            case 4:
                // Execute behavior for button 4
                break;
            case 5:
                // Execute behavior for button 5
                break;
            case 6:
                // Execute behavior for button 6
                break;
            case 7:
                // Execute behavior for button 7
                break;
            case 8:
                // Execute behavior for button 8
                break;
            case 9:
                // Execute behavior for button 9
                break;
            default:
                GD.Print("Unhandled button press");
                break;
        }
    }

    protected override void HandleButtonDisengaged(int buttonIndex)
    {
        GD.Print($"Button {buttonIndex} was released in FeedingStation.");

        switch (buttonIndex)
        {
            case 0:
                // Execute behavior for button 0
                break;
            case 1:
                // Execute behavior for button 1
                break;
            case 2:
                // Execute behavior for button 2
                break;
            case 3:
                // Execute behavior for button 3
                break;
            case 4:
                // Execute behavior for button 4
                break;
            case 5:
                // Execute behavior for button 5
                break;
            case 6:
                // Execute behavior for button 6
                break;
            case 7:
                // Execute behavior for button 7
                break;
            case 8:
                // Execute behavior for button 8
                break;
            case 9:
                // Execute behavior for button 9
                break;
            default:
                GD.Print("Unhandled button press");
                break;
        }
    }

    public void AddSlimeToCanister(double delta, float hungerLevel, float maxHunger, float cleanlinessLevel, float maxCleanliness, float happinessLevel, float maxHappiness)
    {
        if (!canisterInSlot || valveIsOpen) { return; }

        // Start harvesting audio
        if (!hasStartedAudio)
        {
            slimeCollectionAudioNode.Play();
            hasStartedAudio = true;
        }

        // Calculate slime amounts based on hunger, cleanliness, and happiness
        float slimeFromFood = AdjustSlimeRate(hungerLevel, maxHunger, feedingSlimeMultiplier);
        float slimeFromCleanliness = AdjustSlimeRate(cleanlinessLevel, maxCleanliness, cleaningSlimeMultiplier);
        float slimeFromHappiness = AdjustSlimeRate(happinessLevel, maxHappiness, happinessSlimeMultiplier);

        // Total slime added during this frame
        float totalSlimeToAdd = (slimeFromFood + slimeFromCleanliness + slimeFromHappiness) * (float)delta;
        currentSlimeLevel = Mathf.Min(currentSlimeLevel + totalSlimeToAdd, maxSlimeInCanister);

        // Update UI
        stationNeedsProgressBarComponentNode.SetProgressBarValue(currentSlimeLevel);
        debugUI.UpdateCurrentSlimeProgressBar(currentSlimeLevel);

        // If canister is full, trigger full event
        if (currentSlimeLevel >= maxSlimeInCanister)
        {
            stationAlertComponentNode.TriggerStationAlert();
        }
    }

    private float AdjustSlimeRate(float level, float maxLevel, float multiplier)
    {
        float percentage = (level / maxLevel) * 100.0f;

        if (percentage > 100.0f)
        {
            return baseSlimeCollectionRate * multiplier * 1.2f; // Bonus for exceeding the maximum
        }
        if (percentage >= 80.0f)
        {
            return baseSlimeCollectionRate * multiplier; // Boost when above 80%
        }
        if (percentage < 20.0f)
        {
            return baseSlimeCollectionRate * 0.5f; // Penalty when below 20%
        }

        return baseSlimeCollectionRate; // Normal rate
    }

    private void TryAddingBarrelToStation()
    {
        if (playerIsHoldingBarrel && !canisterInSlot)
        {
            canisterInSlot = true;
            canisterMeshToAppear.Visible = true;

            playerIsHoldingBarrel = false;
            globalSignals.RaiseSlimeCanisterAddedToStation();
            GD.Print("New canister added");
        }
    }

    private void HandleValveTargetReached(bool isValveOpen)
    {
        GD.Print($"Signal received: valve is open: {isValveOpen}");
        valveIsOpen = isValveOpen;

        if (valveIsOpen)
        {
            // Remove canister if one is present
            if (canisterInSlot)
            {
                RemoveCanisterFromStationAndBankSlime();
                stationAlertComponentNode.SilenceStationAlert();
            }
        }
    }

    private void RemoveCanisterFromStationAndBankSlime()
    {
        // Stop harvesting audio
        if (hasStartedAudio)
        {
            slimeCollectionAudioNode.Stop();
            hasStartedAudio = false;
        }

        // Play barrel removed audio
        barrelRemovedAudioNode.Play();

        canisterInSlot = false;
        canisterMeshToAppear.Visible = false;
        globalSignals.RaiseSlimeCanisterRemovedFromStation(currentSlimeLevel);
        currentSlimeLevel = 0.0f;

        // Reset current slime display to zero
        stationNeedsProgressBarComponentNode.ResetProgressBar();
        debugUI.UpdateCurrentSlimeProgressBar(currentSlimeLevel);

        // Can play canister removal animation here
    }

    private void HandleSlimeCanisterTakenFromStorage()
    {
        playerIsHoldingBarrel = true;
    }

    private void HandlePlayerClockedOut()
    {
        // Reset barrel and valve
        valveNode.ResetValve();

        canisterInSlot = true;
        valveIsOpen = false;
        canisterMeshToAppear.Visible = true;

        playerIsHoldingBarrel = false;

        currentSlimeLevel = 0.0f;
    }
}
