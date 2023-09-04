using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using System.Collections.Generic;

namespace CraftingRPG.Entities;

public abstract class BaseFetchQuest : IQuest
{
    public abstract Dictionary<ItemId, int> GetRequiredItems();

    public abstract string GetDescription();

    public abstract string GetName();
}
