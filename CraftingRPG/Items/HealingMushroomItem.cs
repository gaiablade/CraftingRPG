using CraftingRPG.Constants;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using System.Collections.Generic;

namespace CraftingRPG.Items;

public class HealingMushroomItem : IItem
{
    public ItemId GetId() => ItemId.HealingMushroom;

    public ISet<ItemCategory> GetItemCategories() => new HashSet<ItemCategory>
    {
        ItemCategory.Ingredient
    };

    public string GetName() => "Healing Mushroom";

    public int GetSpriteSheetIndex() => SpriteIndex.HealingMushroom;
}
