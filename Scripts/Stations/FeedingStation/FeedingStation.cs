using Godot;
using System.Collections.Generic;

public partial class FeedingStation : Station
{
    [ExportCategory("Required Nodes")]
    [Export] Buttons buttonsNode = null;

    private Dictionary<E_IngredientList, bool> activeIngredients;

    public override void _Ready()
    {
        base._Ready();
        GD.Print("Feed station ready test");
    }

    public override void EnterStation()
    {
        GD.Print($"Calling EnterStation method on {Name}");
    }

    public override void ExitStation()
    {
        GD.Print($"Calling ExitStation method on {Name}");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (Input.IsActionJustPressed(GlobalConstants.INPUT_NUM_0))
        {
            PushButton(0);
        }
        if (Input.IsActionJustPressed(GlobalConstants.INPUT_NUM_1))
        {
            PushButton(1);
        }
        if (Input.IsActionJustPressed(GlobalConstants.INPUT_NUM_2))
        {
            PushButton(2);
        }
        if (Input.IsActionJustPressed(GlobalConstants.INPUT_NUM_3))
        {
            PushButton(3);
        }
        if (Input.IsActionJustPressed(GlobalConstants.INPUT_NUM_4))
        {
            PushButton(4);
        }
        if (Input.IsActionJustPressed(GlobalConstants.INPUT_NUM_5))
        {
            PushButton(5);
        }
        if (Input.IsActionJustPressed(GlobalConstants.INPUT_NUM_6))
        {
            PushButton(6);
        }
        if (Input.IsActionJustPressed(GlobalConstants.INPUT_NUM_7))
        {
            PushButton(7);
        }
        if (Input.IsActionJustPressed(GlobalConstants.INPUT_NUM_8))
        {
            PushButton(8);
        }
        if (Input.IsActionJustPressed(GlobalConstants.INPUT_NUM_9))
        {
            PushButton(9);
        }
    }

    private void PushButton(int buttonToPush)
    {
        buttonsNode.PushButton(buttonToPush);
    }
}