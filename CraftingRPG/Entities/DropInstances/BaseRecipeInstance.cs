using CraftingRPG.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace CraftingRPG.Entities.DropInstances;

public abstract class BaseRecipeInstance : IDropInstance
{
    public virtual Vector2 GetPosition()
    {
        throw new System.NotImplementedException();
    }

    public virtual Vector2 SetPosition(Vector2 position)
    {
        throw new System.NotImplementedException();
    }

    public virtual double GetDepth()
    {
        throw new System.NotImplementedException();
    }

    public virtual Vector2 GetSize()
    {
        throw new System.NotImplementedException();
    }

    public virtual RectangleF GetCollisionBox()
    {
        throw new System.NotImplementedException();
    }

    public virtual Texture2D GetSpriteSheet()
    {
        throw new System.NotImplementedException();
    }

    public virtual Rectangle GetTextureRectangle()
    {
        throw new System.NotImplementedException();
    }

    public virtual bool CanDrop()
    {
        throw new System.NotImplementedException();
    }

    public virtual IDroppable GetDroppable()
    {
        throw new System.NotImplementedException();
    }

    public virtual void OnObtain()
    {
    }

    protected void AddToRecipeBook<T>() where T : IRecipe, new()
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