using System.Collections.Generic;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;

namespace CraftingRPG.RecipeManagement;

public class RecipeBook
{
    public Dictionary<RecipeId, IRecipe> Recipes { get; private set; } = new();
    public Dictionary<RecipeId, int> NumberCrafted { get; private set; } = new();

    public bool HasRecipe(RecipeId id) => Recipes.ContainsKey(id);

    public void AddRecipe(RecipeId id, IRecipe recipe)
    {
        Recipes.Add(id, recipe);
        NumberCrafted.Add(id, 0);
    }

    public void AddRecipe(IRecipe recipe)
    {
        Recipes.Add(recipe.GetId(), recipe);
        NumberCrafted.Add(recipe.GetId(), 0);
    }
}
