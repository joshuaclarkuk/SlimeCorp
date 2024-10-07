using Godot;
using System;

public partial class Handicam : Camera3D
{
    [ExportCategory("Camera Movement")]
    // [Export] private float decay = 0.5f; // Removed so that camera never stops moving. Add back in for traditional camera shake
    [Export] private float amplitude = 2.0f;

    private float trauma = 0.5f;
    private float traumaPower = 2.0f;
    private float noiseSpeed = 0.05f;
    private float noiseY = 0.0f;

    private FastNoiseLite noise;

    public override void _Ready()
    {
        // Overrides player camera in Main Menu
        MakeCurrent();

        noise = new FastNoiseLite();
        noise.Seed = 1;
        noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
    }

    public override void _PhysicsProcess(double delta)
    {
        noiseY += noiseSpeed;
        Shake();

        // OLD CAMERA SHAKE METHOD. USE THIS FOR VIOLENT SHAKE
        //if (trauma > 0.0f)
        //{
        //    trauma = Mathf.Max(trauma - decay * (float)delta, 0.0f);
        //    noiseY += noiseSpeed;
        //    Shake();
        //}
    }

    private void AddTrauma(float amount)
    {
        trauma = Mathf.Min(trauma + amount, 1.0f);
    }

    private void Shake()
    {
        float amount = Mathf.Pow(trauma, traumaPower);
        HOffset = amplitude * amount * noise.GetNoise2D(noise.Seed, noiseY);
        VOffset = amplitude * amount * noise.GetNoise2D(noise.Seed * 2, noiseY);
    }
}
