using CraftingRPG.Constants;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;

namespace CraftingRPG.Entities;

public class RecipeDrop<T> : IDroppable where T : IRecipe, new()
{
    public string GetName() => new T().GetName() + " (Recipe)";

    public int GetSpriteSheetIndex() => SpriteIndex.RecipeDrop;

    public void OnObtain()
    {
        var recipe = new T();
        var player = GameManager.PlayerInfo;

        if (!player.RecipeBook.Recipes.ContainsKey(recipe.GetId()))
        {
            player.RecipeBook.AddRecipe(recipe);
        }
    }
}
