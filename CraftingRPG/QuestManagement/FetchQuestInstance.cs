using System.Collections.Generic;
using System.Linq;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;

namespace CraftingRPG.QuestManagement;

public class FetchQuestInstance : BaseQuestInstance
{
    private Dictionary<ItemId, int> CollectedItems;

    public FetchQuestInstance(IFetchQuestInfo fetchQuestInfo)
    {
        QuestInfo = fetchQuestInfo;
        CollectedItems = new();

        foreach (var (itemId, _) in fetchQuestInfo.GetRequiredItems())
        {
            CollectedItems.Add(itemId, 0);
        }
    }

    public IFetchQuestInfo GetFetchQuestInfo() => (IFetchQuestInfo)QuestInfo;

    public void AddCollectedItem(ItemId itemId, int qty)
    {
        if (CollectedItems.ContainsKey(itemId))
        {
            CollectedItems[itemId] += qty;
        }
    }

    public override bool IsComplete() => CollectedItems
        .All(x => GetFetchQuestInfo().GetRequiredItems()[x.Key] <= x.Value);   

    public Dictionary<ItemId, int> GetCollectedItems() => CollectedItems;
}
