public partial class EmailItemSpawner : ComputerItemSpawner
{
    public override void AddNewItemToScreen(ComputerItemResource resourceToAdd)
    {
        EmailItem newItem = (EmailItem)packedSceneToInstantiate.Instantiate();
        newItem.UpdateStringsFromResource(resourceToAdd);
        AddChild(newItem);
    }
}