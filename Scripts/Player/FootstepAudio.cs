using Godot;
using System;

public partial class FootstepAudio : Node3D
{
    [ExportCategory("Audiostream Players")]
    [Export] private AudioStreamPlayer3D concreteFootstepPlayerNode = null;

    [ExportCategory("Audiostreams")]
    [Export] private AudioStream[] concreteFootsteps = null;

    [ExportCategory("RequiredNodes")]
    [Export] private Player playerNode = null;

    [ExportCategory("Behaviour")]
    [Export] private float modifier = 0.5f;

    private AudioStream lastStream = null;
    private float currentSpeed = 0.0f;
    private float distanceCovered = 0.0f;
    private float airTime = 0.0f;
    private bool isWalking = false;

    public override void _Process(double delta)
    {
        currentSpeed = GetPlayerSpeed();
        isWalking = CheckIfWalking();
        // PlaySoundIfFalling((float)delta);

        if (isWalking)
        {
            distanceCovered += (currentSpeed * (float)delta) * modifier;
            if (distanceCovered > 1)
            {
                TriggerNextClip();
                distanceCovered = 0;
            }
        }
    }

    private float GetPlayerSpeed()
    {
        float speed = playerNode.Velocity.Length();
        return speed;
    }

    private bool CheckIfWalking()
    {
        if (currentSpeed > 0 && playerNode.IsOnFloor())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private AudioStream GetStreamFromArray(AudioStream[] streamArray)
    {
        int attempts = 3;
        AudioStream selectedStream = streamArray[GD.RandRange(0, streamArray.Length - 1)];

        while (selectedStream == lastStream && attempts > 0)
        {
            selectedStream = streamArray[GD.RandRange(0, streamArray.Length - 1)];
            attempts--;
        }

        lastStream = selectedStream;
        return selectedStream;
    }

    private void TriggerNextClip()
    {
        concreteFootstepPlayerNode.PitchScale = (float)GD.RandRange(0.9f, 1.1f);
        concreteFootstepPlayerNode.VolumeDb = (float)GD.RandRange(-2.0f, 0.0f);

        if (playerNode.IsOnFloor())
        {
            concreteFootstepPlayerNode.Stream = GetStreamFromArray(concreteFootsteps);
            concreteFootstepPlayerNode.Play();
        }
    }

    private void PlaySoundIfFalling(float delta)
    {
        if (!playerNode.IsOnFloor())
        {
            airTime += delta;
        }
        else
        {
            if (airTime > 0.5f)
            {
                TriggerNextClip();
                airTime = 0;
            }
        }
    }
}
