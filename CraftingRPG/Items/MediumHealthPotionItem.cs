using CraftingRPG.Constants;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;

namespace CraftingRPG.Items;

public class MediumHealthPotionItem : IItem
{
    public ItemId GetId() => ItemId.MediumHealthPotion;

    public string GetName() => "Medium Health Potion";

    public int GetSpriteSheetIndex() => SpriteIndex.MediumHealthPotion;
}
