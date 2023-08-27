using CraftingRPG.Constants;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;

namespace CraftingRPG.Items;

public class SmallHealthPotionItem : IItem
{
    public ItemId GetId() => ItemId.SmallHealthPotion;

    public string GetName() => "Small Health Potion";

    public int GetSpriteSheetIndex() => SpriteIndex.SmallHealthPotion;
}
