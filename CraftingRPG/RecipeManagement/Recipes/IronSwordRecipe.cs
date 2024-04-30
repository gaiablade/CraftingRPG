using System.Collections.Generic;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;

namespace CraftingRPG.RecipeManagement.Recipes;

public class IronSwordRecipe : IRecipe
{
    public ItemId GetCraftedItem() => ItemId.IronSword;

    public RecipeId GetId() => RecipeId.IronSword;

    public Dictionary<ItemId, int> GetIngredients() => new Dictionary<ItemId, int>
    {
        { ItemId.IronChunk, 3 }
    };

    public string GetName() => "Iron Sword";
}
