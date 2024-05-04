using CraftingRPG.AssetManagement;
using CraftingRPG.Global;
using CraftingRPG.Graphics;
using CraftingRPG.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace CraftingRPG.Entities.DropInstances;

public abstract class BaseRecipeInstance : IDropInstance
{
    protected IRecipe Recipe { get; set; }
    protected IDroppable Droppable { get; set; }
    protected Vector2 Position { get; set; }
    protected Point Size { get; set; }
    protected double Depth { get; set; } = -1;

    #region Constructors
    protected BaseRecipeInstance()
    {
        Size = new(16, 16);
    }

    protected BaseRecipeInstance(Point size)
    {
        Size = size;
    }
    #endregion
    
    #region Required Overrides

    public SpriteDrawingData GetDrawingData()
    {
        return new SpriteDrawingData
        {
            Texture = GetSpriteSheet(),
            SourceRectangle = GetTextureRectangle()
        };
    }

    public abstract Rectangle GetTextureRectangle();
    public Vector2 GetMovementVector() => Vector2.Zero;

    #endregion
    
    public virtual Vector2 GetPosition() => Position;
    public virtual IDroppable GetDroppable() => Droppable;
    public Vector2 Move(Vector2 movementVector) => SetPosition(Vector2.Add(GetPosition(), movementVector));
    public virtual double GetDepth() => Depth;
    public virtual Point GetSize() => Size;
    public virtual RectangleF GetCollisionBox() => new(Position, Size);
    public virtual Vector2 SetPosition(Vector2 position) => Position = position;
    public virtual Texture2D GetSpriteSheet() => Assets.Instance.IconSpriteSheet;
    public virtual bool CanDrop() => !Globals.Player.GetInfo().RecipeBook.Recipes.ContainsKey(Recipe.GetId());

    public virtual void OnObtain()
    {
        AddToRecipeBook(Recipe);
    }

    protected void AddToRecipeBook<T>() where T : IRecipe, new()
    {
        AddToRecipeBook(new T());
    }
    
    protected static void AddToRecipeBook(IRecipe recipe)
    {
        var player = Globals.PlayerInfo;

        Assets.Instance.RecipeGrabSfx01.Play(0.3F, 0F, 0F);

        if (!player.RecipeBook.Recipes.ContainsKey(recipe.GetId()))
        {
            player.RecipeBook.AddRecipe(recipe);
        }
    }
}