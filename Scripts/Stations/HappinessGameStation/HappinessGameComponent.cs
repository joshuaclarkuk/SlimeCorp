using Godot;
using System;

public partial class HappinessGameComponent : Node2D
{
    [ExportCategory("Required Nodes")]
    [Export] private HappinessGamePlayer happinessGamePlayerNode = null;
    [Export] private Timer spawnEnemyBlocksTimerNode = null;
    [Export] private HappinessGameEnemy enemyToSpawn = null;
    [Export] private PackedScene projectileSceneToSpawn = null;

    [ExportCategory("Behaviour")]
    [Export] private float moveStep = 32.0f;
    [Export] private float projectileSpawnOffsetY = 32.0f;

    private Vector2 currentPos = Vector2.Zero;
    private Vector2 viewportDimensions = new Vector2(512, 512);
    private float xOffset = 0.0f;
    private float yOffset = 10.0f;
    private int xIndex = 0;
    private int maxXIndex = 15;

    // Enemy spawn variables
    private HappinessGameEnemy[] enemiesToSpawn = null;
    private int maxEnemiesToSpawn = 15;

    private GlobalSignals globalSignals = null;

    public override void _Ready()
    {
        globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");

        // Set xOffset so it doesn't start slightly off screen
        xOffset = moveStep / 2.0f;

        happinessGamePlayerNode.Position = new Vector2(xOffset, viewportDimensions.Y - yOffset);

        // Initialise enemy array
        foreach (HappinessGameEnemy enemy in enemiesToSpawn)
        {
            //enemy[] = 
        }

        // Subscribe to spawn timeout and start timer
        spawnEnemyBlocksTimerNode.Timeout += HandleSpawnEnemyBlocksTimerTimeout;
        spawnEnemyBlocksTimerNode.Start();
    }

    public override void _ExitTree()
    {
        spawnEnemyBlocksTimerNode.Timeout -= HandleSpawnEnemyBlocksTimerTimeout;
    }

    public void MoveOnX(bool isMovingRight)
    {
        if (isMovingRight)
        {
            if (xIndex >= maxXIndex) { return; }

            currentPos.X += moveStep;
            xIndex++;
        }
        else
        {
            if (xIndex <= 0) { return; }

            currentPos.X -= moveStep;
            xIndex--;
        }

        happinessGamePlayerNode.Position = new Vector2(currentPos.X + xOffset, 500.0f);
    }

    public void SpawnProjectile()
    {
        HappinessGameProjectile projectile = (HappinessGameProjectile)projectileSceneToSpawn.Instantiate();
        AddChild(projectile);
        projectile.SetGlobalSignals(globalSignals);
        projectile.Position = new Vector2(happinessGamePlayerNode.Position.X, happinessGamePlayerNode.Position.Y - projectileSpawnOffsetY);
    }

    private void HandleSpawnEnemyBlocksTimerTimeout()
    {
        
    }
}
