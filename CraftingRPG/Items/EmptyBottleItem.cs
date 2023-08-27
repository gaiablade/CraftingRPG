using CraftingRPG.Constants;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;

namespace CraftingRPG.Items
{
    public class EmptyBottleItem : IItem
    {
        public ItemId GetId() => ItemId.EmptyBottle;

        public string GetName() => "Empty Bottle";

        public int GetSpriteSheetIndex() => SpriteIndex.EmptyBottle;
    }
}
