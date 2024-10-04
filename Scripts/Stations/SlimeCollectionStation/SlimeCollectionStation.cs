using Godot;
using System;

public partial class SlimeCollectionStation : Station
{
    [ExportCategory("Required Nodes")]
    [Export] private Valve valveNode = null;
    [Export] private Node3D canisterMeshToAppear = null;
    [Export] private AudioStreamPlayer3D slimeCollectionAudioNode = null;

    [ExportCategory("External Nodes")]
    [Export] private DebugUI debugUI = null;

    [ExportCategory("Slime Collection")]
    [Export] private float baseSlimeCollectionRate = 0.2f;
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

        canisterMeshToAppear.Visible = true;
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        globalSignals.OnSlimeCanisterTakenFromStorage -= HandleSlimeCanisterTakenFromStorage;
    }

    public override void EnterStation()
    {
        base.EnterStation();

        valveNode.OnValveTargetReached += HandleValveTargetReached;

        TryAddingBarrelToStation();
    }

    public override void ExitStation()
    {
        base.ExitStation();

        valveNode.OnValveTargetReached -= HandleValveTargetReached;
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

    public void AddSlimeToCanister(double delta, float creatureHungerLevel, float creatureMaxHungerLevel, float creatureCleanlinessLevel, float creatureMaxCleanlinessLevel, float creatureHappinessLevel, float creatureMaxHappinessLevel)
    {
        if (!canisterInSlot || valveIsOpen) { return; }

        // Play harvesting audio
        if (!hasStartedAudio)
        {
            slimeCollectionAudioNode.Play();
            hasStartedAudio = true;
        }

        // Slime to add from food, cleanliness, and happiness
        float slimeToAddFromFood = baseSlimeCollectionRate;
        float slimeToAddFromCleanliness = baseSlimeCollectionRate;
        float slimeToAddFromHappiness = baseSlimeCollectionRate;

        float hungerPercentage = (creatureHungerLevel / creatureMaxHungerLevel) * 100.0f;
        switch (hungerPercentage)
        {
            case > 100.0f:
                slimeToAddFromFood *= feedingSlimeMultiplier * 1.2f;  // Above max hunger, add bonus
                break;
            case >= 80.0f:
                slimeToAddFromFood *= feedingSlimeMultiplier;  // Above 80%, multiply
                break;
            case < 20.0f:
                slimeToAddFromFood *= 0.5f; // Below 20% incur penalty
                break;
            default:
                slimeToAddFromFood *= 1.0f;  // Normal collection rate
                break;
        }

        float cleanlinessPercentage = (creatureCleanlinessLevel / creatureMaxCleanlinessLevel) * 100.0f;
        switch (cleanlinessPercentage)
        {
            case > 100.0f:
                slimeToAddFromCleanliness *= cleaningSlimeMultiplier * 1.2f;  // Above max hunger, add bonus
                break;
            case >= 80.0f:
                slimeToAddFromCleanliness *= cleaningSlimeMultiplier;  // Above 80%, multiply
                break;
            case < 20.0f:
                slimeToAddFromCleanliness *= 0.5f; // Below 20% incur penalty
                break;
            default:
                slimeToAddFromCleanliness *= 1.0f;  // Normal collection rate
                break;
        }

        float happinessPercentage = (creatureHappinessLevel / creatureMaxHappinessLevel) * 100.0f;
        switch (happinessPercentage)
        {
            case > 100.0f:
                slimeToAddFromHappiness *= happinessSlimeMultiplier * 1.2f;  // Above max hunger, add bonus
                break;
            case >= 80.0f:
                slimeToAddFromHappiness *= happinessSlimeMultiplier;  // Above 80%, multiply
                break;
            case < 20.0f:
                slimeToAddFromHappiness *= 0.5f; // Below 20% incur penalty
                break;
            default:
                slimeToAddFromHappiness *= 1.0f;  // Normal collection rate
                break;
        }

        float totalSlimeToAdd = (slimeToAddFromFood + slimeToAddFromCleanliness + slimeToAddFromHappiness) * (float)delta;
        currentSlimeLevel = Mathf.Min(currentSlimeLevel + totalSlimeToAdd, maxSlimeInCanister); // Ensure it doesn't exceed max capacity

        debugUI.UpdateSlimeProgressBar(currentSlimeLevel);

        // Check if canister is full
        if (currentSlimeLevel >= maxSlimeInCanister)
        {
            // SLIME CANISTER MAXED OUT
            // Alert player to maxed out canister
        }
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

        canisterInSlot = false;
        canisterMeshToAppear.Visible = false;
        globalSignals.RaiseSlimeCanisterRemovedFromStation(currentSlimeLevel);
        currentSlimeLevel = 0.0f;
        debugUI.UpdateSlimeProgressBar(currentSlimeLevel);
        // Can play canister removal animation here
    }

    private void HandleSlimeCanisterTakenFromStorage()
    {
        playerIsHoldingBarrel = true;
    }
}
