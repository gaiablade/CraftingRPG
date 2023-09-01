using CraftingRPG.Constants;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;

namespace CraftingRPG.Items;

public class ArcaneFlowerItem : IItem
{
    public ItemId GetId() => ItemId.ArcaneFlower;

    public string GetName() => "Arcane Flower";

    public int GetSpriteSheetIndex() => SpriteIndex.QuestionMark;
}
