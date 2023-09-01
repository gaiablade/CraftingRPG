using CraftingRPG.Constants;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;

namespace CraftingRPG.Items;

public class MageBraceletItem : IItem
{
    public ItemId GetId() => ItemId.MageBracelet;

    public string GetName() => "Mage Bracelet";

    public int GetSpriteSheetIndex() => SpriteIndex.QuestionMark;
}
