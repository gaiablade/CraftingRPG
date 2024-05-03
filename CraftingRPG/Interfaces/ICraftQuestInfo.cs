using System.Collections.Generic;
using CraftingRPG.Enums;

namespace CraftingRPG.Interfaces;

public interface ICraftQuestInfo : IQuestInfo
{
    public Dictionary<IItem, int> GetRequiredItemsToCraft();
}