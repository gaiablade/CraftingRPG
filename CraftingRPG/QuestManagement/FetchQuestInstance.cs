using System.Collections.Generic;
using System.Linq;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;

namespace CraftingRPG.QuestManagement;

public class FetchQuestInstance : BaseQuestInstance
{
    private IDictionary<ItemId, ItemData> CollectedItems;

    public FetchQuestInstance(IFetchQuestInfo fetchQuestInfo)
    {
        QuestInfo = fetchQuestInfo;
        CollectedItems = new Dictionary<ItemId, ItemData>();

        foreach (var (itemInfo, _) in fetchQuestInfo.GetRequiredItems())
        {
            var id = itemInfo.GetId();
            CollectedItems.Add(id, new ItemData
            {
                ItemInfo = itemInfo,
                Quantity = 0
            });
        }
    }

    public IFetchQuestInfo GetFetchQuestInfo() => (IFetchQuestInfo)QuestInfo;

    public void AddCollectedItem(IItem itemInfo, int qty)
    {
        var id = itemInfo.GetId();
        if (CollectedItems.ContainsKey(id))
        {
            CollectedItems[id].Quantity += qty;
        }
    }

    public override bool IsComplete()
    {
        var requiredItems = GetFetchQuestInfo().GetRequiredItems();
        return requiredItems.All(x => CollectedItems[x.Key.GetId()].Quantity >= x.Value);
    }

    public int GetCollectedItemQuantity(IItem item) => CollectedItems[item.GetId()].Quantity;

    private class ItemData
    {
        public IItem ItemInfo;
        public int Quantity;
    }
}