using CraftingRPG.Constants;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using System;
using System.Collections.Generic;

namespace CraftingRPG.Items;

public class IronChunkItem : IItem
{
    public ItemId GetId() => ItemId.IronChunk;

    public ISet<ItemCategory> GetItemCategories() => new HashSet<ItemCategory>
    {
        ItemCategory.Ingredient
    };

    public string GetName() => "Iron Chunk";

    public int GetSpriteSheetIndex() => SpriteIndex.IronChunk;
}
