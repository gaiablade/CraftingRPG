using System.Collections.Generic;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;

namespace CraftingRPG.QuestManagement.Quests;

public class SmallHealthPotionCraftQuestInfo : ICraftQuestInfo
{
    public string GetName() => "Craft a Small Health Potion";

    public string GetDescription() => "Craft 1 Small Health Potion";

    public Dictionary<ItemId, int> GetRequiredItemsToCraft() => new()
    {
        { ItemId.SmallHealthPotion, 1 }
    };
}