using CraftingRPG.Enums;
using CraftingRPG.Global;
using CraftingRPG.Interfaces;
using CraftingRPG.Recipes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace CraftingRPG.Entities.DropInstances;

public class IronHelmetRecipeInstance : BaseRecipeInstance
{
    private IronHelmetRecipe Recipe;
    private Vector2 Position;
    private Vector2 Size = new(16, 16);

    public IronHelmetRecipeInstance()
    {
        Recipe = new();
    }
    
    public override Vector2 GetPosition()
    {
        return Position;
    }

    public override Vector2 SetPosition(Vector2 position)
    {
        Position = position;
        return position;
    }

    public override double GetDepth()
    {
        return -1;
    }

    public override Vector2 GetSize()
    {
        return Size;
    }

    public override RectangleF GetCollisionBox()
    {
        return new RectangleF(Position, Size);
    }

    public override Texture2D GetSpriteSheet()
    {
        return GameManager.SpriteSheet;
    }

    public override Rectangle GetTextureRectangle()
    {
        return new(0, 512, 32, 32);
    }

    public override bool CanDrop()
    {
        return !Globals.Instance.Player.Info.RecipeBook.Recipes.ContainsKey(RecipeId.IronHelmet);
    }

    public override IDroppable GetDroppable()
    {
        return new RecipeDrop<IronHelmetRecipe>();
    }

    public override void OnObtain()
    {
        AddToRecipeBook<IronHelmetRecipe>();
    }
}