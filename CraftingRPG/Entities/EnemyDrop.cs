using System;
using CraftingRPG.Interfaces;

namespace CraftingRPG.Entities;

public struct EnemyDrop
{
    public int DropRate { get; set; }
    public Func<IDropInstance> CreateDropInstance;

    public EnemyDrop(int dropRate, Func<IDropInstance> createDropInstance)
    {
        this.DropRate = dropRate;
        this.CreateDropInstance = createDropInstance;
    }
}
