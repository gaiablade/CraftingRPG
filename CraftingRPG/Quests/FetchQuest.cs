using System.Collections.Generic;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;

namespace CraftingRPG.Quests;

public abstract class BaseFetchQuest : IQuest
{
    public abstract Dictionary<ItemId, int> GetRequiredItems();

    public abstract string GetDescription();

    public abstract string GetName();
}
