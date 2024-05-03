using CraftingRPG.Enums;
using System.Collections.Generic;

namespace CraftingRPG.Interfaces;

public interface IRecipe
{
    public RecipeId GetId();
    public IDictionary<IItem, int> GetIngredients();
    public IItem GetCraftedItem();

    public string GetName();
}
