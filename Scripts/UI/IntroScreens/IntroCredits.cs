using Godot;
using System;

public partial class IntroCredits : Control
{
    [ExportCategory("Required Nodes")]
    [Export] private AnimationPlayer animationPlayer = null;

    public void PlayIntroCreditsAnimation()
    {
        Visible = true;
        animationPlayer.Play("CreditScrawl");
        animationPlayer.AnimationFinished += HandleAnimationFinished;
    }

    private void HandleAnimationFinished(StringName animName)
    {
        if (animName == "CreditScrawl")
        {
            Visible = false;
            animationPlayer.AnimationFinished -= HandleAnimationFinished;
        }
    }
}
