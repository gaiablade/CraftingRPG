using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using System.Collections.Generic;

namespace CraftingRPG.Recipes;

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
