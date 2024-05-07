using CraftingRPG.Interfaces;

namespace CraftingRPG.Player;

public class Equipment
{
    public IWeapon Weapon { get; set; } = null;
    public IItem Helmet { get; set; } = null;
    public IItem Chestplate { get; set; } = null;
    public IItem Pants { get; set; } = null;
    public IItem Accessory1 { get; set; } = null;
    public IItem Accessory2 { get; set; } = null;
}
