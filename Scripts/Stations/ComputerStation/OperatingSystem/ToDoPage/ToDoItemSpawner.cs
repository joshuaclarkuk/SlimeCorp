public partial class ToDoItemSpawner : ComputerItemSpawner
{
    public override void AddNewItemToScreen(ComputerItemResource resourceToAdd)
    {
        ToDoItem newItem = (ToDoItem)packedSceneToInstantiate.Instantiate();
        newItem.UpdateStringsFromResource(resourceToAdd);
        AddChild(newItem);
    }
}