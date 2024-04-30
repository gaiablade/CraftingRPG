using System.Collections.Generic;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;

namespace CraftingRPG.RecipeManagement.Recipes;

public class MageBraceletRecipe : IRecipe
{
    public ItemId GetCraftedItem() => ItemId.MageBracelet;

    public RecipeId GetId() => RecipeId.MageBracelet;

    public Dictionary<ItemId, int> GetIngredients() => new Dictionary<ItemId, int>
    {
        { ItemId.IronBand, 1 },
        { ItemId.ArcaneFlower, 2 }
    };

    public string GetName() => "Mage Bracelet";
}
