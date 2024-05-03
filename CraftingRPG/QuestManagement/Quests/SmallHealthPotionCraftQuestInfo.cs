using System.Collections.Generic;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using CraftingRPG.Items;

namespace CraftingRPG.QuestManagement.Quests;

public class SmallHealthPotionCraftQuestInfo : ICraftQuestInfo
{
    public string GetName() => "Craft a Small Health Potion";

    public string GetDescription() => "Craft 1 Small Health Potion";

    public Dictionary<IItem, int> GetRequiredItemsToCraft() => new()
    {
        { SmallHealthPotionItem.Instance, 1 }
    };
}