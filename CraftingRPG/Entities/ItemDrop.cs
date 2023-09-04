using CraftingRPG.Interfaces;

namespace CraftingRPG.Entities;

public class ItemDrop<T> : IDroppable where T : IItem, new()
{
    public string GetName() => new T().GetName();

    public int GetSpriteSheetIndex() => new T().GetSpriteSheetIndex();

    public void OnObtain()
    {
        var item = new T();
        var player = GameManager.PlayerInfo;

        player.Inventory[item.GetId()]++;
    }
}
