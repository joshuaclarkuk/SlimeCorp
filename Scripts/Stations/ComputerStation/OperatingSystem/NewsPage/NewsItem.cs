using Godot;

public partial class NewsItem : ComputerItem
{
    public override void UpdateStringsFromResource(ComputerItemResource resource)
    {
        ItemTitle = resource.ItemTitle;
        titleLabelNode.Text = ItemTitle;

        ItemBody = resource.ItemBody;
        bodyLabelNode.Text = ItemBody;

        ItemByline = resource.ItemByline;
        bylineLabelNode.Text = ItemByline;

        ItemDateReceived = resource.ItemDateReceived;
        dateReceivedLabelNode.Text = ItemDateReceived;
    }
}