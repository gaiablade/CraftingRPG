using CraftingRPG.Constants;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using System.Collections.Generic;

namespace CraftingRPG.Items;

public class IronBandItem : IItem
{
    public ItemId GetId() => ItemId.IronBand;

    public ISet<ItemCategory> GetItemCategories() => new HashSet<ItemCategory>
    { 
        ItemCategory.Ingredient 
    };

    public string GetName() => "Iron Band";

    public int GetSpriteSheetIndex() => SpriteIndex.QuestionMark;
}
