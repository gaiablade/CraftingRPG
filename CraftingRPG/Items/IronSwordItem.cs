using CraftingRPG.Constants;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;

namespace CraftingRPG.Items;

public class IronSwordItem : IItem
{
    public ItemId GetId() => ItemId.IronSword;

    public string GetName() => "Iron Sword";

    public int GetSpriteSheetIndex() => SpriteIndex.IronSword;
}
