using CraftingRPG.Enums;
using System.Collections.Generic;

namespace CraftingRPG.Interfaces;

public interface IRecipe
{
    public RecipeId GetId();
    public Dictionary<ItemId, int> GetIngredients();
    public ItemId GetCraftedItem();
}
