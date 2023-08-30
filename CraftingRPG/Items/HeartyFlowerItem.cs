using CraftingRPG.Constants;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using System.Collections.Generic;

namespace CraftingRPG.Items;

public class HeartyFlowerItem : IItem
{
    public ItemId GetId() => ItemId.HeartyFlower;

    public ISet<ItemCategory> GetItemCategories() => new HashSet<ItemCategory>
    {
        ItemCategory.Ingredient
    };

    public string GetName() => "Hearty Flower";

    public int GetSpriteSheetIndex() => SpriteIndex.HeartyFlower;
}
