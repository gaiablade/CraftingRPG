using CraftingRPG.Entities;
using CraftingRPG.Enums;
using System.Collections.Generic;

namespace CraftingRPG.Quests;

public class FetchQuestMushrooms : BaseFetchQuest
{
    public override string GetDescription() => "Sample Description. Blah Blah Blah Blah Blah Blah. Go get the 'shrooms :)";

    public override string GetName() => "Obtain 10 Healing Mushrooms!";

    public override Dictionary<ItemId, int> GetRequiredItems() => new()
    {
        { ItemId.HealingMushroom, 10 }
    };
}
