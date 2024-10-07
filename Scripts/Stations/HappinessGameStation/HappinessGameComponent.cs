using Godot;
using System;

public partial class HappinessGameComponent : Node2D
{
    [ExportCategory("Required Nodes")]
    [Export] private HappinessGamePlayer happinessGamePlayerNode = null;
    [Export] private Timer spawnEnemyBlocksTimerNode = null;
    [Export] private PackedScene enemyToSpawn = null;
    [Export] private PackedScene projectileSceneToSpawn = null;

    [ExportCategory("Behaviour")]
    [Export] private float moveStep = 32.0f;
    [Export] private float projectileSpawnOffsetY = 32.0f;

    private Vector2 currentPos = Vector2.Zero;
    private Vector2 viewportDimensions = new Vector2(512, 512);
    private float xOffset = 0.0f;
    private float yOffset = 16.0f;
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
        enemiesToSpawn = new HappinessGameEnemy[maxEnemiesToSpawn];
        for (int i = 0; i < maxEnemiesToSpawn; i++)
        {
            HappinessGameEnemy enemyInstance = enemyToSpawn.Instantiate<HappinessGameEnemy>();
            enemiesToSpawn[i] = enemyInstance;
        }

        // Subscribe to spawn timeout, start timer and spawn first wave
        spawnEnemyBlocksTimerNode.Timeout += HandleSpawnEnemyBlocksTimerTimeout;
        spawnEnemyBlocksTimerNode.Start();

        // Add enemies to tree
        foreach (HappinessGameEnemy enemy in enemiesToSpawn)
        {
            AddChild(enemy);
        }

        // Set initial positions
        SpawnEnemyWave();
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

        happinessGamePlayerNode.Position = new Vector2(currentPos.X + xOffset, viewportDimensions.Y - yOffset);
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
        SpawnEnemyWave();
    }

    private void SpawnEnemyWave()
    {
        foreach (HappinessGameEnemy enemy in enemiesToSpawn)
        {
            enemy.Position = new Vector2((GD.RandRange(1, 15) * moveStep) + moveStep / 2, (GD.RandRange(1, 8) * moveStep) + moveStep / 2);  // Random spawn position at the top of the screen
            enemy.ToggleEnemy(true); // Set enemy visible to true and activate collider
        }

        spawnEnemyBlocksTimerNode.Start();
    }
}
