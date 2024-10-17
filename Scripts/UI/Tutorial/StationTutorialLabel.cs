using Godot;
using System;

public partial class StationTutorialLabel : Label
{
    [ExportCategory("Station Type")]
    [Export] public E_StationType StationType { get; private set; }
}
