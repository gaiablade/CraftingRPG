using CraftingRPG.Constants;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using CraftingRPG.Recipes;
using Microsoft.Xna.Framework;

namespace CraftingRPG.Entities;

public class PlayerInfo : IPlayerInfo
{
    public RecipeBook RecipeBook { get; private set; } = new();
    public Inventory Inventory { get; private set; } = new();

    public PlayerInfo()
    {
        RecipeBook.AddRecipe(new SmallHealthPotionRecipe());
        RecipeBook.AddRecipe(new MediumHealthPotionRecipe());
        RecipeBook.AddRecipe(new IronSwordRecipe());

        Inventory[ItemId.EmptyBottle] = 4;
        Inventory[ItemId.HealingMushroom] = 10;
        Inventory[ItemId.IronChunk] = 15;
    }
}
