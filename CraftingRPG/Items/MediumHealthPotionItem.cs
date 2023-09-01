using CraftingRPG.Constants;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using System.Collections.Generic;

namespace CraftingRPG.Items;

public class MediumHealthPotionItem : IItem
{
    public ItemId GetId() => ItemId.MediumHealthPotion;

    public ISet<ItemCategory> GetItemCategories() => new HashSet<ItemCategory>
    {
        ItemCategory.Potion
    };

    public string GetName() => "Medium Health Potion";

    public int GetSpriteSheetIndex() => SpriteIndex.MediumHealthPotion;
}
