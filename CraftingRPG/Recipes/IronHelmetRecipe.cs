using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using System.Collections.Generic;

namespace CraftingRPG.Recipes;

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
