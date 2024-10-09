using Godot;

[GlobalClass]
public partial class DailyNeedResource : Resource
{
    [ExportCategory("Shift Times and Titles")]
    [Export(PropertyHint.MultilineText)] public string TitleToDisplay { get; private set; } = "Day Unknown";
    [Export] public int MinutesInDay { get; private set; } = 5;
    [Export] public float SlimeCollectionTarget { get; private set; } = 100.0f;

    [ExportCategory("Depletion Rates")]
    [Export] public float HungerDepletionRate { get; private set; } = 0.2f;
    [Export] public float HappinessDepletionRate { get; private set; } = 0.15f;
    [Export] public float CleanlinessDepletionRate { get; private set; } = 0.1f;

    [ExportCategory("Replenishment/Addition Rates")]
    [Export] public float MaxHungerReplenishment { get; private set; } = 100.0f;
    [Export] public float MaxCleanlinessReplenishment { get; private set; } = 75.0f;
    [Export] public float MaxHappinessReplenishment { get; private set; } = 5.0f;
    [Export] public float MaxWasteProductToAdd { get; private set; } = 10.0f;
}