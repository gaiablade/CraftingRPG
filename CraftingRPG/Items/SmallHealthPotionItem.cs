﻿using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using System.Collections.Generic;
using CraftingRPG.AssetManagement;
using CraftingRPG.Constants;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CraftingRPG.Items;

public class SmallHealthPotionItem : IItem
{
    public static SmallHealthPotionItem Instance = new();
    
    public ItemId GetId() => ItemId.SmallHealthPotion;

    public ISet<ItemCategory> GetItemCategories() => new HashSet<ItemCategory>
    {
        ItemCategory.Potion, ItemCategory.Ingredient
    };

    public Texture2D GetTileSet()
    {
        return Assets.Instance.IconSpriteSheet;
    }

    public Rectangle GetSourceRectangle()
    {
        return new Rectangle(160, 544, 32, 32);
    }

    public string GetName() => ItemName.SmallHealthPotion;
}
