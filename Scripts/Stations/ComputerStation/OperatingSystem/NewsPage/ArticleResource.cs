using Godot;
using System;

public partial class ArticleResource : Resource
{
    [Export] private string articleTitle = string.Empty;
    [Export(PropertyHint.MultilineText)] private string articleDescription = string.Empty;
}
