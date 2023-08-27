using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using CraftingRPG.Recipes;
using Microsoft.Xna.Framework;

namespace CraftingRPG.Entities;

public class Player : IPlayer
{
    public RecipeBook RecipeBook { get; private set; } = new();
    public Inventory Inventory { get; private set; } = new();

    #region Movement
    public const int MovementSpeed = 5;
    public Point Position = new Point();
    #endregion

    public Player()
    {
        RecipeBook.AddRecipe(new SmallHealthPotionRecipe());
        RecipeBook.AddRecipe(new MediumHealthPotionRecipe());
        RecipeBook.AddRecipe(new IronSwordRecipe());

        Inventory[ItemId.EmptyBottle] = 4;
        Inventory[ItemId.HealingMushroom] = 10;
        Inventory[ItemId.IronChunk] = 15;
    }
}
