using CraftingRPG.Constants;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using System.Collections.Generic;

namespace CraftingRPG.Items
{
    public class EmptyBottleItem : IItem
    {
        public ItemId GetId() => ItemId.EmptyBottle;

        public ISet<ItemCategory> GetItemCategories() => new HashSet<ItemCategory>
        {
            { ItemCategory.Ingredient }
        };

        public string GetName() => "Empty Bottle";

        public int GetSpriteSheetIndex() => SpriteIndex.EmptyBottle;
    }
}
