using CraftingRPG.Enums;
using System;
using System.Collections.Generic;

namespace CraftingRPG.Entities;

public class GlobalFlags
{
    public Dictionary<ChestId, bool> IsChestOpen;

    public GlobalFlags()
    {
        IsChestOpen = new();
        for (var i = 0; i < Enum.GetValues(typeof(ChestId)).Length; i++)
        {
            var id = (ChestId)i;
            IsChestOpen.Add(id, false);
        }
    }
}
