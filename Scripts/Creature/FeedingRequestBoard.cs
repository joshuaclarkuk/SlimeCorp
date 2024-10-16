using Godot;
using System;
using System.Collections.Generic;

public partial class FeedingRequestBoard : Node3D
{
    [ExportCategory("Eldritch Sprites")]
    [Export] private PackedScene[] eldritchSpritesScenes = null;

    private EldritchSprite[] eldritchSprites = new EldritchSprite[9];
    private GlobalSignals globalSignals = null;
    private List<E_IngredientList> activeIngredients = new List<E_IngredientList>();

    private float newSpriteOffsetX = -4.0f;

    public override void _Ready()
    {
        globalSignals = GetNode<GlobalSignals>("/root/GlobalSignals");
        globalSignals.OnCreatureFeedRequest += HandleCreatureFeedRequest;
        globalSignals.OnServeCreatureFood += HandleServeCreatureFood;
        globalSignals.OnPlayerClockedOut += HandlePlayerClockedOut;

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
        globalSignals.OnPlayerClockedOut -= HandlePlayerClockedOut;
    }

    private void HandleCreatureFeedRequest(List<E_IngredientList> list)
    {
        activeIngredients.Clear();

        foreach (E_IngredientList ingredient in list)
        {
            activeIngredients.Add(ingredient);
        }

        ClearExistingList();
        PopulateNewList(activeIngredients);
    }

    private void HandleServeCreatureFood(List<E_IngredientList> list)
    {
        foreach (E_IngredientList ingredient in list)
        {
            if (activeIngredients.Contains(ingredient))
            {
                activeIngredients.Remove(ingredient);
            }
        }

        ClearExistingList();
        PopulateNewList(activeIngredients);
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
        for (int i = 0; i < activeIngredients.Count; i++)
        {
            for (int j = 0; j < eldritchSprites.Length; j++)
            {
                if (eldritchSprites[j] != null && eldritchSprites[j].IngredientType == activeIngredients[i])
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

    private void HandlePlayerClockedOut()
    {
        ClearExistingList();
        activeIngredients.Clear();
    }
}
