using System.Collections.Generic;
using CraftingRPG.Constants;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using CraftingRPG.Items;

namespace CraftingRPG.RecipeManagement.Recipes;

public class MediumHealthPotionRecipe : BaseRecipe
{
    public override RecipeId GetId() => RecipeId.MediumHealthPotion;

    public override IDictionary<IItem, int> GetIngredients() => new Dictionary<IItem, int>
    {
        { EmptyBottleItem.Instance, 1 },
        { HealingMushroomItem.Instance, 1 },
        { HeartyFlowerItem.Instance, 1 }
    };

    public override IItem GetCraftedItem() => MediumHealthPotionItem.Instance;

    public override string GetName() => ItemName.MediumHealthPotion;
}
