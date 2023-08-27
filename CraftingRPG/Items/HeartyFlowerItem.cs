using CraftingRPG.Constants;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;

namespace CraftingRPG.Items;

public class HeartyFlowerItem : IItem
{
    public ItemId GetId() => ItemId.HeartyFlower;

    public string GetName() => "Hearty Flower";

    public int GetSpriteSheetIndex() => SpriteIndex.HeartyFlower;
}
