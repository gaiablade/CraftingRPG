using System.Collections.Generic;
using CraftingRPG.Enums;

namespace CraftingRPG.QuestManagement.Quests;

public class FetchQuestSmallPotionIngredients : BaseFetchQuest
{
    public override string GetDescription() => "Sample Description. Blah Blah Blah Blah Blah Blah. Go get the 'shrooms :)";

    public override string GetName() => "Obtain 10 Healing Mushrooms!";

    public override Dictionary<ItemId, int> GetRequiredItems() => new()
    {
        { ItemId.HealingMushroom, 2 },
        { ItemId.EmptyBottle, 1 }
    };
}
