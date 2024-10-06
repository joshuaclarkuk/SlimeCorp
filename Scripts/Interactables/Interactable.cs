using Godot;

public abstract partial class Interactable : Area3D
{
    protected GlobalValues globalValues = null;

    public override void _Ready()
    {
        globalValues = GetNode<GlobalValues>("/root/GlobalValues");
    }

    public abstract void Interact();
}