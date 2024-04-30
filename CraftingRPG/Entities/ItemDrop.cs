using CraftingRPG.Interfaces;

namespace CraftingRPG.Entities;

public class ItemDrop<T> : IDroppable where T : IItem, new()
{
    public string GetName() => new T().GetName();
}
