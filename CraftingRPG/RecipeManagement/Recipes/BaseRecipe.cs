using System.Collections.Generic;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;

namespace CraftingRPG.RecipeManagement.Recipes;

public abstract  class BaseRecipe : IRecipe
{
    public abstract RecipeId GetId();

    public abstract IDictionary<IItem, int> GetIngredients();

    public abstract IItem GetCraftedItem();

    public abstract string GetName();
}