using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using System.Collections.Generic;
using CraftingRPG.AssetManagement;
using CraftingRPG.Constants;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CraftingRPG.Items;

public class MageBraceletItem : IItem
{
    public ItemId GetId() => ItemId.MageBracelet;

    public ISet<ItemCategory> GetItemCategories() => new HashSet<ItemCategory>
    {
        ItemCategory.Accessory
    };

    public Texture2D GetTileSet()
    {
        return Assets.Instance.IconSpriteSheet;
    }

    public Rectangle GetSourceRectangle()
    {
        return new Rectangle(0, 1568, 32, 32);
    }

    public string GetName() => ItemName.MageBracelet;
}
