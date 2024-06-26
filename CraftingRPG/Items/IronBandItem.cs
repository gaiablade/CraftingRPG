﻿using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using System.Collections.Generic;
using CraftingRPG.AssetManagement;
using CraftingRPG.Constants;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CraftingRPG.Items;

public class IronBandItem : IItem
{
    public static readonly IronBandItem Instance = new();
    
    public ItemId GetId() => ItemId.IronBand;

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
        return new Rectangle(0, 1600, 0, 0);
    }

    public string GetName() => ItemName.IronBand;
}
