using Godot;

public partial class HappinessGameStation : Station
{
    [ExportCategory("Required Nodes")]
    [Export] private HappinessGameComponent happinessGameComponentNode = null;
    [Export] private Sprite3D screenNode = null;

    [ExportCategory("Behaviour")]
    private float fadeDuration = 0.5f;

    public override void _Ready()
    {
        base._Ready();

        screenNode.Modulate = new Color(0.0f, 0.0f, 0.0f, 1.0f);
    }

    public override void EnterStation()
    {
        base.EnterStation();

        FadeScreen(new Color(1.0f, 1.0f, 1.0f, 1.0f));

        GD.Print($"Calling EnterStation method on {Name}");
    }

    public override void ExitStation()
    {
        base.ExitStation();

        FadeScreen(new Color(0.0f, 0.0f, 0.0f, 1.0f));

        GD.Print($"Calling ExitStation method on {Name}");
    }

    protected override void HandleButtonEngaged(int buttonIndex)
    {
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
                PressMovementKey(4);
                break;
            case 5:
                // Execute behavior for button 5
                break;
            case 6:
                // Execute behavior for button 6
                PressMovementKey(6);
                break;
            case 7:
                // Execute behavior for button 7
                break;
            case 8:
                // Execute behavior for button 8
                PressMovementKey(8);
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

    private void PressMovementKey(int digit)
    {
        switch (digit)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                happinessGameComponentNode.MoveOnX(false);
                break;
            case 5:
                break;
            case 6:
                happinessGameComponentNode.MoveOnX(true);
                break;
            case 7:
                break;
            case 8:
                happinessGameComponentNode.SpawnProjectile();
                break;
            case 9:
                break;
        }
    }

    private void FadeScreen(Color finalValue)
    {
        Tween fadeTween = CreateTween();
        fadeTween.SetTrans(Tween.TransitionType.Sine);
        fadeTween.SetEase(Tween.EaseType.InOut);
        fadeTween.TweenProperty(screenNode, "modulate", finalValue, fadeDuration);
    }
}