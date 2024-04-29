using CraftingRPG.Interfaces;
using CraftingRPG.Quests;

namespace CraftingRPG.Entities;

public class ItemDrop<T> : IDroppable where T : IItem, new()
{
    public bool CanDrop() => true;

    public string GetName() => new T().GetName();

    public void OnObtain()
    {
        var item = new T();
        var player = GameManager.PlayerInfo;

        player.Inventory[item.GetId()]++;
        GameManager.MaterialGrabSfx01.Play(0.3F, 0F, 0F);

        foreach (var questInstance in player.Quests)
        {
            if (questInstance is FetchQuestInstance fetchQuestInstance)
            {
                fetchQuestInstance.AddCollectedItem(item.GetId(), 1);
            }
        }
    }
}
