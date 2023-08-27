using CraftingRPG.Constants;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;

namespace CraftingRPG.Items;

public class IronHelmetItem : IItem
{
    public ItemId GetId() => ItemId.IronHelmet;

    public string GetName() => "Iron Helmet";

    public int GetSpriteSheetIndex() => SpriteIndex.QuestionMark;
}
