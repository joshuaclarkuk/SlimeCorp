using Godot;
using System;

public partial class OneShotAudioComponent : Node3D
{
    [ExportCategory("Audiostream Players")]
    [Export] private AudioStreamPlayer3D audioPlayerNode = null;

    [ExportCategory("Audiostreams")]
    [Export] private AudioStream[] audioStreams = null;

    [ExportCategory("Behaviour")]
    [Export] private float minPitchRange = 0.9f;
    [Export] private float maxPitchRange = 1.1f;
    [Export] private float minVolumeDb = -6.0f;
    [Export] private float maxVolumeDb = -2.0f;

    private AudioStream lastStream = null;


    public void PlayAudioClip()
    {
        audioPlayerNode.PitchScale = (float)GD.RandRange(minPitchRange, maxPitchRange);
        audioPlayerNode.VolumeDb = (float)GD.RandRange(minVolumeDb, maxVolumeDb);

        audioPlayerNode.Stream = GetStreamFromArray(audioStreams);
        audioPlayerNode.Play();
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
}
