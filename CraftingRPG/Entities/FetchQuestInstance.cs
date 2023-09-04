using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace CraftingRPG.Entities;

public class FetchQuestInstance : IQuestInstance
{
    private BaseFetchQuest Instance;
    private Dictionary<ItemId, int> CollectedItems;

    public FetchQuestInstance(BaseFetchQuest fetchQuest)
    {
        Instance = fetchQuest;
        CollectedItems = new();

        foreach (var (itemId, _) in Instance.GetRequiredItems())
        {
            CollectedItems.Add(itemId, 0);
        }
    }

    public IQuest GetQuest() => Instance;

    public void AddCollectedItem(ItemId itemId, int qty)
    {
        if (CollectedItems.ContainsKey(itemId))
        {
            CollectedItems[itemId] += qty;
        }
    }

    public bool IsComplete() => CollectedItems.All(x => Instance.GetRequiredItems()[x.Key] <= x.Value);
}
