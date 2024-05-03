using System.Collections.Generic;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using CraftingRPG.Items;

namespace CraftingRPG.RecipeManagement.Recipes;

public class MageBraceletRecipe : BaseRecipe
{
    public override RecipeId GetId() => RecipeId.MageBracelet;

    public override IDictionary<IItem, int> GetIngredients() => new Dictionary<IItem, int>
    {
        { IronBandItem.Instance, 1 },
        { ArcaneFlowerItem.Instance, 2 }
    };

    public override IItem GetCraftedItem() => MageBraceletItem.Instance;

    public override string GetName()
    {
        throw new System.NotImplementedException();
    }
}
