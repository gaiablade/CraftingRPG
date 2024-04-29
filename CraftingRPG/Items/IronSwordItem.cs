using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using System.Collections.Generic;
using CraftingRPG.AssetManagement;
using CraftingRPG.Constants;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CraftingRPG.Items;

public class IronSwordItem : IWeapon
{
    public int GetAttackStat() => 20;

    public ItemId GetId() => ItemId.IronSword;

    public ISet<ItemCategory> GetItemCategories() => new HashSet<ItemCategory>
    {
        ItemCategory.Weapon
    };

    public Texture2D GetTileSet()
    {
        return Assets.Instance.IconSpriteSheet;
    }

    public Rectangle GetSourceRectangle()
    {
        return new Rectangle(0, 864, 32, 32);
    }

    public string GetName() => ItemName.IronSword;
}
