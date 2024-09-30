using Godot;
using System;

public partial class Buttons : Node3D
{
    [ExportCategory("Required Nodes")]
    [Export] private Button[] buttonArray = null;

    [ExportCategory("Button Behaviour")]
    [Export] private bool shouldStayDown = false;
    [Export] private float buttonPressDuration = 0.3f;

    private float travelAmount = 0.04f;

    private bool isTravelling = false;

    public event Action<int> OnButtonEngaged;
    public event Action<int> OnButtonDisengaged;

    public override void _Ready()
    {
        foreach (Button button in buttonArray)
        {
            button.OnButtonDowned += HandleButtonIsDown;
        }
    }

    public void PushButton(int buttonNumber)
    {
        // Guard clause to make sure you can't press a button that's animating
        if (buttonArray[buttonNumber].IsTravelling) { return; }

        if (!buttonArray[buttonNumber].IsDown)
        {
            buttonArray[buttonNumber].DepressButton(travelAmount, buttonPressDuration);
        }
        else
        {
            buttonArray[buttonNumber].RaiseButton(buttonPressDuration);
        }
    }

    private void HandleButtonIsDown(Button button, bool isDown)
    {
        // Get array position of button pressed
        int buttonIndex = Array.IndexOf(buttonArray, button);

        if (isDown)
        {
            OnButtonEngaged?.Invoke(buttonIndex);
        }
        else
        {
            OnButtonDisengaged?.Invoke(buttonIndex);
        }
    }
}
