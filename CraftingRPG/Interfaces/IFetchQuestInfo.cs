using System.Collections.Generic;
using CraftingRPG.Enums;

namespace CraftingRPG.Interfaces;

public interface IFetchQuestInfo : IQuestInfo
{
    public Dictionary<ItemId, int> GetRequiredItems();
}