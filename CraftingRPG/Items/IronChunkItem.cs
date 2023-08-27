using CraftingRPG.Constants;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using System;

namespace CraftingRPG.Items;

public class IronChunkItem : IItem
{
    public ItemId GetId() => ItemId.IronChunk;

    public string GetName() => "Iron Chunk";

    public int GetSpriteSheetIndex() => SpriteIndex.IronChunk;
}
