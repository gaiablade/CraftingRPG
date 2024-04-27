using CraftingRPG.Interfaces;

namespace CraftingRPG.Entities;

public class RecipeDrop<T> : IDroppable where T : IRecipe, new()
{
    public bool CanDrop() => !GameManager.PlayerInfo.RecipeBook.Recipes.ContainsKey(new T().GetId());

    public string GetName() => new T().GetName() + " (Recipe)";

    public void OnObtain()
    {
        var recipe = new T();
        var player = GameManager.PlayerInfo;

        GameManager.RecipeGrabSfx01.Play(0.3F, 0F, 0F);
        
        if (!player.RecipeBook.Recipes.ContainsKey(recipe.GetId()))
        {
            player.RecipeBook.AddRecipe(recipe);
        }
    }
}
