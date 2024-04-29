using CraftingRPG.Constants;
using CraftingRPG.Entities;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using System.Collections.Generic;
using CraftingRPG.Entities.DropInstances;

namespace CraftingRPG.Enemies;

public class GreenSlime : IEnemy
{
    public List<EnemyDrop> GetDropTable() => new()
    {
        new EnemyDrop(100, () => new HealingMushroomInstance()),
        new EnemyDrop(80, () => new IronHelmetRecipeInstance())
    };

    public EnemyId GetId() => EnemyId.GreenSlime;

    public string GetName() => EnemyNames.GreenSlime;

    public int GetMaxHitPoints() => 50;
}
