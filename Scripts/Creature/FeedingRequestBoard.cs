using Godot;
using System;
using System.Collections.Generic;

public partial class FeedingRequestBoard : Node3D
{
    [ExportCategory("Eldritch Sprites")]
    [Export] private PackedScene[] eldritchSpritesScenes = null;

    private EldritchSprite[] eldritchSprites = new EldritchSprite[9];
    private GlobalSignals globalSignals = null;

    private float newSpriteOffsetX = -4.0f;

    public override void _Ready()
    {
        globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");
        globalSignals.OnCreatureFeedRequest += HandleCreatureFeedRequest;
        globalSignals.OnServeCreatureFood += HandleServeCreatureFood;

        // Populate eldritch sprites array
        for (int i = 0; i < eldritchSpritesScenes.Length; i++)
        {
            if (eldritchSpritesScenes[i] != null)
            {
                eldritchSprites[i] = (EldritchSprite)eldritchSpritesScenes[i].Instantiate();
            }
        }
    }

    public override void _ExitTree()
    {
        globalSignals.OnCreatureFeedRequest -= HandleCreatureFeedRequest;
        globalSignals.OnServeCreatureFood -= HandleServeCreatureFood;
    }

    private void HandleCreatureFeedRequest(List<E_IngredientList> list)
    {
        ClearExistingList();
        PopulateNewList(list);
    }

    private void HandleServeCreatureFood(List<E_IngredientList> list)
    {        
        foreach(Node3D child in GetChildren())
        {
            EldritchSprite eldritchSprite = child.GetChild<EldritchSprite>(0);
            if (list.Contains(eldritchSprite.IngredientType))
            {
                child.QueueFree();
            }
        }
    }

    private void ClearExistingList()
    {
        foreach (Node child in GetChildren())
        {
            child.QueueFree();
        }
    }

    private void PopulateNewList(List<E_IngredientList> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            for (int j = 0; j < eldritchSprites.Length; j++)
            {
                if (eldritchSprites[j].IngredientType == list[i])
                {
                    Node3D newIngredientNode = new Node3D();
                    AddChild(newIngredientNode);

                    EldritchSprite newSpriteInstance = (EldritchSprite)eldritchSprites[j].Duplicate();
                    newIngredientNode.AddChild(newSpriteInstance);

                    newIngredientNode.Position = new Vector3(newSpriteOffsetX * i, 0.0f, 0.0f);

                    break;
                }
            }
        }
    }
}
