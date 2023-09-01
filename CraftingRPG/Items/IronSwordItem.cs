using CraftingRPG.Constants;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using System.Collections.Generic;

namespace CraftingRPG.Items;

public class IronSwordItem : IItem
{
    public ItemId GetId() => ItemId.IronSword;

    public ISet<ItemCategory> GetItemCategories() => new HashSet<ItemCategory>
    {
        ItemCategory.Weapon
    };

    public string GetName() => "Iron Sword";

    public int GetSpriteSheetIndex() => SpriteIndex.IronSword;
}
