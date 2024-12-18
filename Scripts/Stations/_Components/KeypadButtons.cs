using Godot;
using System;

public partial class KeypadButtons : Node3D
{
    [ExportCategory("Required Nodes")]
    [Export] public KeypadButton[] ButtonArray { get; private set; } = null;
    [Export] private OneShotAudioComponent oneShotAudioComponentNode = null;

    [ExportCategory("Button Behaviour")]
    [Export] private bool shouldStayDown = false;
    [Export] private float buttonPressDuration = 0.05f;
    [Export] private float travelAmount = 0.04f;

    private bool isTravelling = false;

    public event Action<int> OnButtonEngaged;
    public event Action<int> OnButtonDisengaged;

    public override void _Ready()
    {
        foreach (KeypadButton button in ButtonArray)
        {
            button.OnButtonDowned += HandleButtonIsPushed;
        }
    }

    public void PushButton(int buttonNumber)
    {
        // Guard clause to make sure you can't press a button that's animating
        if (ButtonArray[buttonNumber].IsTravelling) { return; }

        if (!ButtonArray[buttonNumber].IsDown)
        {
            ButtonArray[buttonNumber].DepressButton(travelAmount, buttonPressDuration);
            if (Visible)
            {
                oneShotAudioComponentNode.PlayAudioClip();
            }
        }
        else
        {
            ButtonArray[buttonNumber].RaiseButton(buttonPressDuration);
        }
    }

    public void ResetAndRaiseAllButtons()
    {
        // Guard clause to ensure only buttons that should stay down can use this method
        if (!shouldStayDown) { return; }

        // Raise all buttons
        foreach (KeypadButton button in ButtonArray)
        {
            button.RaiseButton(buttonPressDuration);
        }
    }

    private void HandleButtonIsPushed(KeypadButton button, bool isDown)
    {
        // Get array position of button pressed
        int buttonIndex = Array.IndexOf(ButtonArray, button);

        if (shouldStayDown)
        {
            if (isDown)
            {
                OnButtonEngaged?.Invoke(buttonIndex);
            }
            else
            {
                OnButtonDisengaged?.Invoke(buttonIndex);
            }
        }
        else
        {
            if (isDown)
            {
                button.RaiseButton(buttonPressDuration);
                OnButtonEngaged?.Invoke(buttonIndex);
            }
        }
    }
}
