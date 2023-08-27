using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using System.Collections.Generic;

namespace CraftingRPG.Entities;

public class RecipeBook
{
    public Dictionary<RecipeId, IRecipe> Recipes { get; private set; }
    public Dictionary<RecipeId, int> NumberCrafted { get; private set; }

    public RecipeBook()
    {
        Recipes = new Dictionary<RecipeId, IRecipe>();
        NumberCrafted = new Dictionary<RecipeId, int>();
    }

    public void AddRecipe(RecipeId Id, IRecipe Recipe)
    {
        Recipes.Add(Id, Recipe);
        NumberCrafted.Add(Id, 0);
    }

    public void AddRecipe(IRecipe recipe)
    {
        Recipes.Add(recipe.GetId(), recipe);
        NumberCrafted.Add(recipe.GetId(), 0);
    }
}
