using CraftingRPG.Constants;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using System.Collections.Generic;

namespace CraftingRPG.Items;

public class MageBraceletItem : IItem
{
    public ItemId GetId() => ItemId.MageBracelet;

    public ISet<ItemCategory> GetItemCategories() => new HashSet<ItemCategory>
    {
        ItemCategory.Accessory
    };

    public string GetName() => "Mage Bracelet";

    public int GetSpriteSheetIndex() => SpriteIndex.QuestionMark;
}
