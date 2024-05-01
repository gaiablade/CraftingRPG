﻿using System.Collections.Generic;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;

namespace CraftingRPG.RecipeManagement;

public class RecipeBook
{
    public Dictionary<RecipeId, IRecipe> Recipes { get; private set; }
    public Dictionary<RecipeId, int> NumberCrafted { get; private set; }

    public RecipeBook()
    {
        Recipes = new Dictionary<RecipeId, IRecipe>();
        NumberCrafted = new Dictionary<RecipeId, int>();
    }

    public bool HasRecipe(RecipeId Id) => Recipes.ContainsKey(Id);

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