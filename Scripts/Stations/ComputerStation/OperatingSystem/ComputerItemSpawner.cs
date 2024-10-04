using Godot;

public abstract partial class ComputerItemSpawner : VBoxContainer
{
    [ExportCategory("Packed Scenes")]
    [Export] protected PackedScene packedSceneToInstantiate = null;

    public abstract void AddNewItemToScreen(ComputerItemResource resourceToAdd);
}
