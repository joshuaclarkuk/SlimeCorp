public partial class NewsItemSpawner : ComputerItemSpawner
{

    public override void AddNewItemToScreen(ComputerItemResource resourceToAdd)
    {
        NewsItem newItem = (NewsItem)packedSceneToInstantiate.Instantiate();
        newItem.UpdateStringsFromResource(resourceToAdd);
        AddChild(newItem);
    }
}