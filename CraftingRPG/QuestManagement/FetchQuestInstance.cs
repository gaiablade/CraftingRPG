using System.Collections.Generic;
using System.Linq;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;

namespace CraftingRPG.QuestManagement;

public class FetchQuestInstance : BaseQuestInstance
{
    private Dictionary<IItem, int> CollectedItems;

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

    public void AddCollectedItem(IItem itemInfo, int qty)
    {
        if (CollectedItems.ContainsKey(itemInfo))
        {
            CollectedItems[itemInfo] += qty;
        }
    }

    public override bool IsComplete() => CollectedItems
        .All(x => GetFetchQuestInfo().GetRequiredItems()[x.Key] <= x.Value);   

    public Dictionary<IItem, int> GetCollectedItems() => CollectedItems;
}
