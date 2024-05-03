using System.Collections.Generic;
using CraftingRPG.Constants;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using CraftingRPG.Items;

namespace CraftingRPG.RecipeManagement.Recipes;

public class IronSwordRecipe : BaseRecipe
{
    public override RecipeId GetId() => RecipeId.IronSword;

    public override IDictionary<IItem, int> GetIngredients() => new Dictionary<IItem, int>()
    {
        { IronChunkItem.Instance, 3 }
    };

    public override IItem GetCraftedItem() => IronSwordItem.Instance;

    public override string GetName() => ItemName.IronSword;
}
