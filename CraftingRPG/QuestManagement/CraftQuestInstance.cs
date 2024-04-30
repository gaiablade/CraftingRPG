using System.Collections.Generic;
using System.Linq;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;

namespace CraftingRPG.QuestManagement;

public class CraftQuestInstance : BaseQuestInstance
{
    private readonly Dictionary<ItemId, int> CraftedItems = new();
    private readonly Dictionary<ItemId, int> RequiredItemsToCraft;

    public CraftQuestInstance(ICraftQuestInfo craftQuestInfo)
    {
        QuestInfo = craftQuestInfo;
        RequiredItemsToCraft = craftQuestInfo.GetRequiredItemsToCraft();

        foreach (var (itemId, _) in RequiredItemsToCraft)
        {
            CraftedItems[itemId] = 0;
        }
    }

    public ICraftQuestInfo GetCraftQuestInfo() => (ICraftQuestInfo)QuestInfo;
    
    public void ItemCrafted(ItemId id)
    {
        if (RequiredItemsToCraft.ContainsKey(id))
        {
            CraftedItems[id]++;
        }
    }

    public int GetCraftedCount(ItemId id) => CraftedItems[id];

    public override bool IsComplete()
    {
        return RequiredItemsToCraft.All(x => CraftedItems[x.Key] >= x.Value);
    }
}