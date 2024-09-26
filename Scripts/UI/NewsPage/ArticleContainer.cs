using Godot;
using System;

public partial class ArticleContainer : VBoxContainer
{
    [Export] private ArticleResource articleResource = null;
    [Export] private Label headerLabel = null;
    [Export] private Label bodyLabel = null;
}
