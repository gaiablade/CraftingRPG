using System;
using System.Collections.Generic;
using CraftingRPG.Enums;

namespace CraftingRPG.Global;

public class Flags
{
    public Dictionary<ChestId, bool> IsChestOpen;

    public Flags()
    {
        IsChestOpen = new();
        for (var i = 0; i < Enum.GetValues(typeof(ChestId)).Length; i++)
        {
            var id = (ChestId)i;
            IsChestOpen.Add(id, false);
        }
    }
}
