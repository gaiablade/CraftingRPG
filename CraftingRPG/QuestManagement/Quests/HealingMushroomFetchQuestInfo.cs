using System.Collections.Generic;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using CraftingRPG.Items;

namespace CraftingRPG.QuestManagement.Quests;

public class HealingMushroomFetchQuestInfo : IFetchQuestInfo
{
    public string GetDescription() => "Sample Description. Collect 10 Healing Mushrooms!";

    public string GetName() => "Obtain 10 Healing Mushrooms!";

    public Dictionary<IItem, int> GetRequiredItems() => new()
    {
        { HealingMushroomItem.Instance, 10 }
    };
}
