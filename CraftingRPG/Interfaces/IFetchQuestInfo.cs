using System.Collections.Generic;

namespace CraftingRPG.Interfaces;

public interface IFetchQuestInfo : IQuestInfo
{
    public Dictionary<IItem, int> GetRequiredItems();
}