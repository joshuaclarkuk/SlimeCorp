using Godot;

public partial class Options : Node
{
    public bool IsInverted { get; private set; } = true;

    public void SetIsInverted(bool isInverted)
    {
        IsInverted = isInverted;
    }
}