using Godot;
using System;

public partial class PoisonCreatureStation : Station
{
    public override void EnterStation()
    {
        base.EnterStation();
    }

    protected override void HandleButtonDisengaged(int buttonIndex)
    {
        throw new NotImplementedException();
    }

    protected override void HandleButtonEngaged(int buttonIndex)
    {
        throw new NotImplementedException();
    }
}
