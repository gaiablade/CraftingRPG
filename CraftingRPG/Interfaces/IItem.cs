using CraftingRPG.Enums;
using System.Collections.Generic;

namespace CraftingRPG.Interfaces;

public interface IItem
{
    public string GetName();
    public ItemId GetId();
    public int GetSpriteSheetIndex();
    public ISet<ItemCategory> GetItemCategories();
}
