using Godot;

[GlobalClass]
public partial class ComputerItemResource : Resource
{
    [Export] public string ItemTitle { get; private set; } = string.Empty;
    [Export(PropertyHint.MultilineText)] public string ItemBody { get; private set; } = string.Empty;
    [Export] public string ItemByline { get; private set; } = string.Empty;
    [Export] public string ItemDateReceived { get; private set; } = string.Empty;
}