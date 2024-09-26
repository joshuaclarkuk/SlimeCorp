using Godot;

public partial class EmailContainer : VBoxContainer
{
    [Export] private EmailResource emailResource = null;
    [Export] private Label headerLabel = null;
    [Export] private Label bodyLabel = null;
}