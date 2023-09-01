using CraftingRPG.Constants;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;

namespace CraftingRPG.Items;

public class IronBandItem : IItem
{
    public ItemId GetId() => ItemId.IronBand;

    public string GetName() => "Iron Band";

    public int GetSpriteSheetIndex() => SpriteIndex.QuestionMark;
}
