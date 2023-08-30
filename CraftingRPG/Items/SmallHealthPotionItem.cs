using CraftingRPG.Constants;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using System.Collections.Generic;

namespace CraftingRPG.Items;

public class SmallHealthPotionItem : IItem
{
    public ItemId GetId() => ItemId.SmallHealthPotion;

    public ISet<ItemCategory> GetItemCategories() => new HashSet<ItemCategory>
    {
        ItemCategory.Potion, ItemCategory.Ingredient
    };

    public string GetName() => "Small Health Potion";

    public int GetSpriteSheetIndex() => SpriteIndex.SmallHealthPotion;
}
