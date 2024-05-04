using System.Collections.Generic;
using System.Linq;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;

namespace CraftingRPG.Entities;

public class Inventory
{
    private IDictionary<ItemId, InventoryItem> ItemQuantities = new SortedDictionary<ItemId, InventoryItem>();
    
    public int GetItemCount<T>() where T : IItem, new()
    {
        var item = new T();
        return GetItemCount(item);
    }

    public int GetItemCount(IItem item)
    {
        var id = item.GetId();
        var found = ItemQuantities.TryGetValue(id, out var inventoryItem);
        return found ? inventoryItem.Quantity : 0;
    }

    public void SetItemCount<T>(int quantity) where T : IItem, new()
    {
        var item = new T();
        var id = item.GetId();
        if (ItemQuantities.TryGetValue(id, out var inventoryItem))
        {
            inventoryItem.Quantity += quantity;
        }
        else
        {
            ItemQuantities.Add(id, new InventoryItem
            {
                Item = item,
                Quantity = quantity
            });
        }
    }
    
    public void SetItemCount(IItem item, int quantity)
    {
        var id = item.GetId();
        if (ItemQuantities.TryGetValue(id, out var inventoryItem))
        {
            inventoryItem.Quantity = quantity;
        }
        else
        {
            ItemQuantities.Add(id, new InventoryItem
            {
                Item = item,
                Quantity = quantity
            });
        }
    }
    
    public void AddQuantity<T>(int quantity) where T : IItem, new() => SetItemCount<T>(GetItemCount<T>() + quantity);

    public void AddQuantity(IItem item, int quantity) => SetItemCount(item, GetItemCount(item) + quantity);

    public IDictionary<ItemId, InventoryItem> GetItems() => ItemQuantities;

    public class InventoryItem
    {
        public IItem Item;
        public int Quantity;
    }
}