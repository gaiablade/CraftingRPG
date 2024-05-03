using System.Collections.Generic;
using System.Linq;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;

namespace CraftingRPG.QuestManagement;

public class CraftQuestInstance : BaseQuestInstance
{
    private readonly Dictionary<IItem, int> CraftedItems = new();
    private readonly Dictionary<IItem, int> RequiredItemsToCraft;

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
    
    public void ItemCrafted(IItem itemInfo)
    {
        if (RequiredItemsToCraft.ContainsKey(itemInfo))
        {
            CraftedItems[itemInfo]++;
        }
    }

    public int GetCraftedCount(IItem itemInfo) => CraftedItems[itemInfo];

    public override bool IsComplete()
    {
        return RequiredItemsToCraft.All(x => CraftedItems[x.Key] >= x.Value);
    }
}