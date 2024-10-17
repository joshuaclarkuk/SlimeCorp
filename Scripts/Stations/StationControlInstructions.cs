using Godot;
using System;

public partial class StationControlInstructions : Control
{
    [ExportCategory("Required Nodes")]
    [Export] private Control numPadIconsNode = null;
    [Export] private Control happinessGameIconsNode = null;
    [Export] private Control mouseDragIconNode = null;

    [ExportCategory("Tutorial Labels")]
    [Export] private StationTutorialLabel[] tutorialLabelNodes = null;

    public void SetNumPadIconsVisible(bool isVisible)
    {
        numPadIconsNode.Visible = isVisible;
    }

    public void SetHappinessGameIconsVisible(bool isVisible)
    {
        happinessGameIconsNode.Visible = isVisible;
    }

    public void SetMouseDragIconVisible(bool isVisible)
    {
        mouseDragIconNode.Visible = isVisible;
    }

    public void ActivateTutorialNode(E_StationType stationType)
    {
        foreach (StationTutorialLabel label in tutorialLabelNodes)
        {
            if (label.StationType == stationType)
            {
                label.Visible = true;
            }
            else
            {
                label.Visible = false;
            }
        }
    }

    public void DisableAllTutorialNodes()
    {
        foreach (StationTutorialLabel label in tutorialLabelNodes)
        {
            label.Visible = false;
        }
    }
}
