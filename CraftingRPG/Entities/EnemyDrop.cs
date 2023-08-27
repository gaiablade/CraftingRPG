using CraftingRPG.Interfaces;

namespace CraftingRPG.Entities;

public struct EnemyDrop
{
    public int DropRate { get; set; }
    public IDroppable Drop { get; set; }

    public EnemyDrop(int dropRate, IDroppable drop)
    {
        this.DropRate = dropRate;
        this.Drop = drop;
    }
}
