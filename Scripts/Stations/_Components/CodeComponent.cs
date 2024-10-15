using Godot;
using System;

public partial class CodeComponent : Node3D
{
    public int[] correctCode = new int[4];

    [ExportCategory("Digit Nodes")]
    [Export] private Label3D[] digits = new Label3D[4];
    [Export] private Color defaultColour = new Color(1, 1, 1, 1);
    [Export] private Color enteredColour = new Color(1, 1, 1, 1);
    [Export] private Color correctColour = new Color(1, 1, 1, 1);
    [Export] private Color incorrectColour = new Color(1, 1, 1, 1);

    [ExportCategory("Required Nodes")]
    [Export] private Timer codeResetTimer = null;

    private int[] codeEntered = new int[4];
    private int currentCodeIndex = 0;

    private bool isReady = true;

    private GlobalValues globalValues = null;

    public event Action<bool> OnCorrectCodeEntered;

    public override void _Ready()
    {
        // Get reference to global values to expose correct employee number
        globalValues = GetNode<GlobalValues>("/root/GlobalValues");

        codeResetTimer.Timeout += HandleCodeResetTimerTimeout;

        ResetCode();
    }

    public override void _ExitTree()
    {
        codeResetTimer.Timeout -= HandleCodeResetTimerTimeout;
    }

    public void SetCorrectCode(int[] code)
    {
        for (int i = 0; i < code.Length; i++)
        {
            correctCode[i] = code[i];
        }
    }

    public void EnterDigitToMachine(int digit)
    {
        if (!isReady) { return; } // Guard clause to prevent digits being entered while code is resetting

        if (currentCodeIndex < codeEntered.Length)
        {
            codeEntered[currentCodeIndex] = digit;
            digits[currentCodeIndex].Text = digit.ToString();
            digits[currentCodeIndex].Modulate = enteredColour;
            currentCodeIndex++;
        }
        else
        {
            GD.PrintErr("Index out of bounds: currentCodeIndex is too large.");
            return;
        }

        // Check code when all four digits are entered
        if (currentCodeIndex >= codeEntered.Length)
        {
            CheckCode();
        }
    }

    private void CheckCode()
    {
        GD.Print("Checking code...");

        isReady = false;

        // Compare codeEntered with employeeNumber
        bool isCorrect = true;

        for (int i = 0; i < codeEntered.Length; i++)
        {
            if (codeEntered[i] != correctCode[i])
            {
                isCorrect = false;
                break;
            }
        }

        if (isCorrect)
        {
            GD.Print("Success: Code entered is correct!");
            OnCorrectCodeEntered?.Invoke(true);
            codeResetTimer.Start();

            // Set all numbers to green
            foreach (Label3D digit in digits)
            {
                digit.Modulate = correctColour;
            }
        }
        else
        {
            GD.Print("Failure: Code entered is incorrect.");
            OnCorrectCodeEntered?.Invoke(false);
            codeResetTimer.Start();

            // Set all numbers to red
            foreach (Label3D digit in digits)
            {
                digit.Modulate = incorrectColour;
            }
        }
    }

    private void HandleCodeResetTimerTimeout()
    {
        ResetCode();

        isReady = true;
    }

    private void ResetCode()
    {
        currentCodeIndex = 0;
        for (int i = 0; i < codeEntered.Length; i++)
        {
            codeEntered[i] = 0;
            digits[i].Text = 0.ToString();
            digits[i].Modulate = defaultColour;
        }
    }
}
