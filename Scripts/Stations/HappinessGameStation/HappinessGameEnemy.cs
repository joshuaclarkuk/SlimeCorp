using Godot;
using System;

public partial class HappinessGameEnemy : CharacterBody2D
{
    [ExportCategory("Required Nodes")]
    [Export] private Area2D enemyHitboxAreaNode = null;

    public override void _EnterTree()
    {
        enemyHitboxAreaNode.AreaEntered += HandleEnemyHitboxAreaEntered;
    }

    public override void _ExitTree()
    {
        enemyHitboxAreaNode.AreaEntered -= HandleEnemyHitboxAreaEntered;
    }

    private void HandleEnemyHitboxAreaEntered(Area2D area)
    {
        if (area is HappinessGameProjectile)
        {
            GD.Print("Good hit");
            HappinessGameProjectile projectile = (HappinessGameProjectile)area;
            projectile.QueueFree();
            QueueFree();
        }
    }
}
