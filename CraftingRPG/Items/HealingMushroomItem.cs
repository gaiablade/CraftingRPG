using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using System.Collections.Generic;
using CraftingRPG.AssetManagement;
using CraftingRPG.Constants;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CraftingRPG.Items;

public class HealingMushroomItem : IItem
{
    public ItemId GetId() => ItemId.HealingMushroom;

    public ISet<ItemCategory> GetItemCategories() => new HashSet<ItemCategory>
    {
        ItemCategory.Ingredient
    };

    public Texture2D GetTileSet()
    {
        return Assets.Instance.IconSpriteSheet;
    }

    public Rectangle GetSourceRectangle()
    {
        return new Rectangle(32, 672, 32, 32);
    }

    public string GetName() => ItemName.HealingMushroom;
}
