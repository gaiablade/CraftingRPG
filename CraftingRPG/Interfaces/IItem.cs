using CraftingRPG.Enums;

namespace CraftingRPG.Interfaces;

public interface IItem
{
    public string GetName();
    public ItemId GetId();
    public int GetSpriteSheetIndex();
}
