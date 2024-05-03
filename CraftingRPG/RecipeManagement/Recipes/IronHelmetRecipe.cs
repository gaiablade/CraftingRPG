using System.Collections.Generic;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using CraftingRPG.Items;

namespace CraftingRPG.RecipeManagement.Recipes;

public class IronHelmetRecipe : BaseRecipe
{
    public override IItem GetCraftedItem() => IronHelmetItem.Instance;

    public override RecipeId GetId() => RecipeId.IronHelmet;

    public override IDictionary<IItem, int> GetIngredients() => new Dictionary<IItem, int>
    {
        { IronChunkItem.Instance, 2 }
    };

    public override string GetName() => "Iron Helmet";
}
