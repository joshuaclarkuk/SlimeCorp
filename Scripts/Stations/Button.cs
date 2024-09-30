using Godot;
using System;

public partial class Button : CsgCylinder3D
{
    public bool IsTravelling { get; private set; } = false;
    public bool IsDown { get; private set; } = false;

    private Vector3 startingPosition = Vector3.Zero;

    public event Action<Button, bool> OnButtonDowned;

    public override void _Ready()
    {
        startingPosition = Position;
    }

    public void DepressButton(float travelAmount, float buttonPressDuration)
    {
        IsTravelling = true;

        Vector3 newPosition = startingPosition;
        newPosition.Y -= travelAmount;

        Tween animateButtonDownTween = GetTree().CreateTween();
        animateButtonDownTween.SetTrans(Tween.TransitionType.Sine);
        animateButtonDownTween.TweenProperty(this, "position", newPosition, buttonPressDuration);
        animateButtonDownTween.Play();
        animateButtonDownTween.Finished += HandleAnimateButtonDownTweenFinished;        
    }

    public void RaiseButton(float buttonPressDuration)
    {
        IsTravelling = true;

        Tween animateButtonUpTween = GetTree().CreateTween();
        animateButtonUpTween.SetTrans(Tween.TransitionType.Sine);
        animateButtonUpTween.TweenProperty(this, "position", startingPosition, buttonPressDuration);
        animateButtonUpTween.Play();
        animateButtonUpTween.Finished += HandleAnimateButtonUpTweenFinished;
    }

    private void HandleAnimateButtonDownTweenFinished()
    {
        IsDown = true;
        OnButtonDowned?.Invoke(this, IsDown); // Sent to Buttons class
        IsTravelling = false;
    }

    private void HandleAnimateButtonUpTweenFinished()
    {
        IsDown = false;
        OnButtonDowned?.Invoke(this, IsDown); // Sent to Buttons class
        IsTravelling = false;
    }
}
