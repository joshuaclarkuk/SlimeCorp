﻿using Godot;

[GlobalClass]
public partial class DailyNeedResource : Resource
{
    [ExportCategory("Shift Times and Titles")]
    [Export(PropertyHint.MultilineText)] public string TitleToDisplay { get; private set; } = "Day Unknown";
    [Export] public int MinutesInDay { get; private set; } = 5;
    [Export] public float SlimeCollectionTarget { get; private set; } = 100.0f;

    [ExportCategory("Depletion Rates")]
    [Export] public float HungerDepletionRate { get; private set; } = 1.0f;
    [Export] public float HappinessDepletionRate { get; private set; } = 1.0f;
    [Export] public float CleanlinessDepletionRate { get; private set; } = 1.0f;
    [Export] public float AngerDepletionRate { get; private set; } = 1.0f;

    [ExportCategory("Replenishment/Addition Rates")]
    [Export] public float MaxHungerReplenishment { get; private set; } = 20.0f;
    [Export] public float MaxHappinessReplenishment { get; private set; } = 5.0f;
    [Export] public float MaxCleanlinessReplenishment { get; private set; } = 20.0f;
    [Export] public float MaxWasteProductToAdd { get; private set; } = 5.0f;
    [Export] public float MaxAngerToAdd { get; private set; } = 5.0f;

    [ExportCategory("Computer Items to Add")]
    [Export] public ComputerItemResource[] ToDoItemResource { get; private set; } = null;
    [Export] public ComputerItemResource[] EmailItemResources { get; private set; } = null;
    [Export] public ComputerItemResource[] NewsItemResources { get; private set; } = null;
}