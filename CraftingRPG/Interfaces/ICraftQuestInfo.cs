using System.Collections.Generic;

namespace CraftingRPG.Interfaces;

public interface ICraftQuestInfo : IQuestInfo
{
    public Dictionary<IItem, int> GetRequiredItemsToCraft();
}