using System.Collections.Generic;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;

namespace CraftingRPG.QuestManagement.Quests;

public class HealingMushroomFetchQuestInfo : IFetchQuestInfo
{
    public string GetDescription() => "Sample Description. Blah Blah Blah Blah Blah Blah. Go get the 'shrooms :)";

    public string GetName() => "Obtain 10 Healing Mushrooms!";

    public Dictionary<ItemId, int> GetRequiredItems() => new()
    {
        { ItemId.HealingMushroom, 10 }
    };
}
