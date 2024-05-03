using System.Collections.Generic;
using CraftingRPG.Constants;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using CraftingRPG.Items;

namespace CraftingRPG.RecipeManagement.Recipes;

public class SmallHealthPotionRecipe : IRecipe
{
    public RecipeId GetId() => RecipeId.SmallHealthPotion;

    public IDictionary<IItem, int> GetIngredients() => new Dictionary<IItem, int>
    {
        { EmptyBottleItem.Instance, 1 },
        { HealingMushroomItem.Instance, 2 }
    };

    public IItem GetCraftedItem() => SmallHealthPotionItem.Instance;

    public string GetName() => ItemName.SmallHealthPotion;
}