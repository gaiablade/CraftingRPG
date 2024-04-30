using System.Collections.Generic;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;

namespace CraftingRPG.RecipeManagement.Recipes;

public class IronHelmetRecipe : IRecipe
{
    public ItemId GetCraftedItem() => ItemId.IronHelmet;

    public RecipeId GetId() => RecipeId.IronHelmet;

    public Dictionary<ItemId, int> GetIngredients() => new Dictionary<ItemId, int>
    {
        { ItemId.IronChunk, 2 }
    };

    public string GetName() => "Iron Helmet";
}
