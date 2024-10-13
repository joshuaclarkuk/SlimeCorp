using Godot;

public abstract partial class EldritchSprite : Sprite3D
{
    [ExportCategory("Identifying Enum")]
    [Export] public E_IngredientList IngredientType { get; private set; }
}