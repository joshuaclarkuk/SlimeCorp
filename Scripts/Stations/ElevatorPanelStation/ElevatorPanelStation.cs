using Godot;

public partial class ElevatorPanelStation : Station
{
    [Export] private CodeComponent codeComponent = null;

    protected override void HandleButtonEngaged(int buttonIndex)
    {
        throw new System.NotImplementedException();
    }

    protected override void HandleButtonDisengaged(int buttonIndex)
    {
        throw new System.NotImplementedException();
    }
}