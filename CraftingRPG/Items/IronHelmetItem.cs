using CraftingRPG.Constants;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using System.Collections.Generic;

namespace CraftingRPG.Items;

public class IronHelmetItem : IItem
{
    public ItemId GetId() => ItemId.IronHelmet;

    public ISet<ItemCategory> GetItemCategories() => new HashSet<ItemCategory>
    {
        ItemCategory.Armor
    };

    public string GetName() => "Iron Helmet";

    public int GetSpriteSheetIndex() => SpriteIndex.QuestionMark;
}
