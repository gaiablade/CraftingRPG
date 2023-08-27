using CraftingRPG.Enums;
using CraftingRPG.Interfaces;

namespace CraftingRPG.Entities;

public class RecipeDrop<T> : IDroppable where T : IRecipe, new()
{
    public void OnObtain()
    {
        var recipe = new T();
        var player = GameManager.Player;

        if (!player.RecipeBook.Recipes.ContainsKey(recipe.GetId()))
        {
            player.RecipeBook.AddRecipe(recipe);
        }
    }
}
