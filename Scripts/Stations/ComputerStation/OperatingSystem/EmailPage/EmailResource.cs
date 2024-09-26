using Godot;

public partial class EmailResource : Resource
{
    [Export] private string emailSubject = string.Empty;
    [Export(PropertyHint.MultilineText)] private string emailBody = string.Empty;

    private string signOff = string.Empty; // REPLACE THIS WITH THE ACTUAL SIGN OFF
}