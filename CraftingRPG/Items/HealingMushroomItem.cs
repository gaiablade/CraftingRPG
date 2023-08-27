using CraftingRPG.Constants;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;

namespace CraftingRPG.Items;

public class HealingMushroomItem : IItem
{
    public ItemId GetId() => ItemId.HealingMushroom;

    public string GetName() => "Healing Mushroom";

    public int GetSpriteSheetIndex() => SpriteIndex.HealingMushroom;
}
