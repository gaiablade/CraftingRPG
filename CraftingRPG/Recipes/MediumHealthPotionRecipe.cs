using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftingRPG.Recipes;

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
}
