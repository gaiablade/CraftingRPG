using CraftingRPG.Interfaces;

namespace CraftingRPG.Entities;

public class ItemDrop<T> : IDroppable where T : IItem, new()
{
    public void OnObtain()
    {
        var item = new T();
        var player = GameManager.Player;

        player.Inventory[item.GetId()]++;
    }
}
