using Godot;
using System;

public partial class HappinessGameEnemy : CharacterBody2D
{
    [ExportCategory("Required Nodes")]
    [Export] private CollisionShape2D characterBodyColliderNode = null;

    public void ToggleEnemy(bool isAlive)
    {
        Visible = isAlive;
        CallDeferred(nameof(ToggleCollider), isAlive);
    }

    private void ToggleCollider(bool isAlive)
    {
        characterBodyColliderNode.Disabled = !isAlive;
    }
}
