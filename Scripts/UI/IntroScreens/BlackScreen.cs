using Godot;
using System;

public partial class BlackScreen : Control
{
    private GlobalSignals globalSignals;

    public override void _Ready()
    {
        globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");

        // Standard intro for each level
        DisplayBlackScreen();
        WaitSecondsAndDisappear(3.0f);
    }

    public void FadeBlackScreen(bool isFadingToBlack, float fadeDuration)
    {
        Tween fadeTween = CreateTween();
        fadeTween.Finished += HandleFadeTweenFinished;
        fadeTween.SetTrans(Tween.TransitionType.Sine);
        fadeTween.SetEase(Tween.EaseType.InOut);

        if (isFadingToBlack)
        {
            fadeTween.TweenProperty(this, "modulate:a", 0.0f, fadeDuration);

        }
        else
        {
            fadeTween.TweenProperty(this, "modulate:a", 1.0f, fadeDuration);

            if (Modulate.A > 0)
            {
                Visible = true;
            }
        }
    }

    public void DisplayBlackScreen()
    {
        if (!Visible)
        {
            Visible = true;
            Modulate = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        }
    }

    public void WaitSecondsAndDisappear(float duration)
    {
        Tween waitTween = CreateTween();
        waitTween.TweenInterval(duration);
        waitTween.TweenCallback(Callable.From(OnWaitFinished));
    }

    private void HandleFadeTweenFinished()
    {
        if (Modulate.A <= 0.0f)
        {
            Visible = false;
        }
    }

    private void OnWaitFinished()
    {
        Visible = false;
        globalSignals.RaiseOnBlackScreenDisappeared();
    }
}
