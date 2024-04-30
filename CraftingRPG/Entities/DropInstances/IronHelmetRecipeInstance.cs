using CraftingRPG.RecipeManagement.Recipes;
using Microsoft.Xna.Framework;

namespace CraftingRPG.Entities.DropInstances;

public class IronHelmetRecipeInstance : BaseRecipeInstance
{
    public IronHelmetRecipeInstance() : base(new Point(16, 16))
    {
        Recipe = new IronHelmetRecipe();
        Droppable = new RecipeDrop<IronHelmetRecipe>();
    }

    public override Rectangle GetTextureRectangle()
    {
        return new(224, 32, 32, 32);
    }

    public override void OnObtain()
    {
        AddToRecipeBook<IronHelmetRecipe>();
    }
}