using System.Collections.Generic;
using CraftingRPG.Interfaces;
using CraftingRPG.Items;

namespace CraftingRPG.QuestManagement.Quests;

public class HealingMushroomFetchQuestInfo : IFetchQuestInfo
{
    public string GetDescription() => "Sample Description. Collect 3 Healing Mushrooms!";

    public string GetName() => "Obtain 3 Healing Mushrooms!";

    public Dictionary<IItem, int> GetRequiredItems() => new()
    {
        { HealingMushroomItem.Instance, 3 }
    };
}
