using CraftingRPG.Enums;
using System.Collections.Generic;

namespace CraftingRPG.Entities;

public class Inventory
{
    public Dictionary<ItemId, int> Items = new();

    public int this[ItemId Id]
    {
        get
        {
            if (Items.ContainsKey(Id))
                return Items[Id];
            return 0;
        }
        set
        {
            if (Items.ContainsKey(Id))
            {
                Items[Id] = value;
            }
            else
            {
                Items.Add(Id, value);
            }
        }
    }
}
