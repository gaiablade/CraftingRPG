using System.Collections.Generic;
using CraftingRPG.Interfaces;

namespace CraftingRPG.Entities;

public class Inventory
{
    public IDictionary<IItem, int> ItemQuantities = new SortedDictionary<IItem, int>(comparer: new ItemComparer());
    
    public int GetItemCount<T>() where T : IItem, new()
    {
        var item = new T();
        var found = ItemQuantities.TryGetValue(item, out var qty);
        return found ? qty : 0;
    }

    public int GetItemCount(IItem item)
    {
        var found = ItemQuantities.TryGetValue(item, out var qty);
        return found ? qty : 0;
    }

    public void SetItemCount<T>(int quantity) where T : IItem, new()
    {
        var item = new T();
        ItemQuantities[item] = quantity;
    }
    
    public void SetItemCount(IItem item, int quantity)
    {
        ItemQuantities[item] = quantity;
    }
    
    public void AddQuantity<T>(int quantity) where T : IItem, new() => SetItemCount<T>(GetItemCount<T>() + quantity);

    public void AddQuantity(IItem item, int quantity) => SetItemCount(item, quantity);

    private class ItemComparer : IComparer<IItem>
    {
        public int Compare(IItem x, IItem y)
        {
            return x?.GetId().CompareTo(y?.GetId()) ?? 0;
        }
    }
}