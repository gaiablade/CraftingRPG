using CraftingRPG.Constants;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using CraftingRPG.Items;
using CraftingRPG.Recipes;
using Microsoft.Xna.Framework;

namespace CraftingRPG.Entities;

public class PlayerInfo : IPlayerInfo
{
    public RecipeBook RecipeBook { get; private set; } = new();
    public Inventory Inventory { get; private set; } = new();
    public PlayerEquipment Equipment { get; private set; } = new();

    public PlayerInfo()
    {
        RecipeBook.AddRecipe(new SmallHealthPotionRecipe());
        RecipeBook.AddRecipe(new MediumHealthPotionRecipe());
        RecipeBook.AddRecipe(new IronSwordRecipe());
        RecipeBook.AddRecipe(new MageBraceletRecipe());

        Inventory[ItemId.EmptyBottle] = 4;
        Inventory[ItemId.HealingMushroom] = 10;
        Inventory[ItemId.IronChunk] = 15;
        Inventory[ItemId.IronSword] = 1;
        Inventory[ItemId.MediumHealthPotion] = 30;
        Inventory[ItemId.SmallHealthPotion] = 100;
        Inventory[ItemId.IronHelmet] = 2;

        Equipment.Weapon = new IronSwordItem();
        Equipment.Helmet = new IronHelmetItem();
    }
}
