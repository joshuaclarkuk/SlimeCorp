using Godot;
using System;

public partial class WarningSiren : Node3D
{
    [ExportCategory("Required Nodes")]
    [Export] private SpotLight3D warningLightNode = null;
    [Export] private AudioStreamPlayer3D warningSoundNode = null;

    [ExportCategory("Behaviour")]
    [Export] private float rotationSpeed = 5.0f;

    private GlobalSignals globalSignals = null;

    public override void _Ready()
    {
        globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");
        globalSignals.OnPlayerAtRiskOfFailing += HandlePlayerAtRiskOfFailing;

        warningLightNode.Visible = false;
        SetProcess(false);
    }

    public override void _ExitTree()
    {
        globalSignals.OnPlayerAtRiskOfFailing -= HandlePlayerAtRiskOfFailing;
    }

    public override void _Process(double delta)
    {
        RotateY(rotationSpeed * (float)delta);
    }

    public void ActivateWarningSiren()
    {
        SetProcess(true);
        warningLightNode.Visible = true;
        warningSoundNode.Play();
    }

    public void DeactivateWarningSiren()
    {
        SetProcess(false);
        warningLightNode.Visible = false;
        warningSoundNode.Stop();
    }

    private void HandlePlayerAtRiskOfFailing(bool atRisk)
    {
        if (atRisk)
        {
            ActivateWarningSiren();
        }
        else
        {
            DeactivateWarningSiren();
        }
    }
}
