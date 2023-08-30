using CraftingRPG.Interfaces;
namespace CraftingRPG.Entities;

public class PlayerEquipment
{
    public IItem Weapon { get; set; } = null;
    public IItem Helmet { get; set; } = null;
    public IItem Chestplate { get; set; } = null;
    public IItem Pants { get; set; } = null;
    public IItem Accessory1 { get; set; } = null;
    public IItem Accessory2 { get; set; } = null;
}
