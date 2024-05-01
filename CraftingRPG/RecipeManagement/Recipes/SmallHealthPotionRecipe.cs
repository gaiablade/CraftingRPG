using System.Collections.Generic;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;

namespace CraftingRPG.RecipeManagement.Recipes;

public class SmallHealthPotionRecipe : IRecipe
{
    public ItemId GetCraftedItem() => ItemId.SmallHealthPotion;

    public RecipeId GetId() => RecipeId.SmallHealthPotion;

    public Dictionary<ItemId, int> GetIngredients() => new Dictionary<ItemId, int>
    {
        { ItemId.EmptyBottle, 1 },
        { ItemId.HealingMushroom, 2 }
    };

    public string GetName() => "Small Health Potion";
}
