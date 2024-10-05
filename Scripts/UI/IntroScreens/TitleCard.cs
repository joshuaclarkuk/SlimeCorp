using Godot;
using System;

public partial class TitleCard : Control
{
    [ExportCategory("Required Nodes")]
    [Export] private Label titleLabelNode = null;
    [Export] private AudioStreamPlayer bangAudioPlayerNode = null;

    [ExportCategory("Behaviour")]
    [Export] private float durationToDisplay = 3.0f;

    public void UpdateTextAndDisplay(string text)
    {
        // Set new text
        titleLabelNode.Text = text;

        // If not visible, set to visible
        if (!Visible)
        {
            Visible = true;
        }

        // Play bang audio
        bangAudioPlayerNode.Play();

        Tween waitTween = CreateTween();
        waitTween.TweenInterval(durationToDisplay);
        waitTween.TweenCallback(Callable.From(DisappearTitleCard));
    }

    private void DisappearTitleCard()
    {
        Visible = false;
    }
}
