using System.Collections.Generic;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;

namespace CraftingRPG.RecipeManagement.Recipes;

public class MediumHealthPotionRecipe : IRecipe
{
    public ItemId GetCraftedItem() => ItemId.MediumHealthPotion;

    public RecipeId GetId() => RecipeId.MediumHealthPotion;

    public Dictionary<ItemId, int> GetIngredients() => new Dictionary<ItemId, int>
    {
        { ItemId.EmptyBottle, 1 },
        { ItemId.HealingMushroom, 1 },
        { ItemId.HeartyFlower, 1 }
    };

    public string GetName() => "Medium Health Potion";
}
