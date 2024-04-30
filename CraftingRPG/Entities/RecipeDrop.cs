using CraftingRPG.Interfaces;

namespace CraftingRPG.Entities;

public class RecipeDrop<T> : IDroppable where T : IRecipe, new()
{
    public string GetName() => new T().GetName() + " (Recipe)";
}
