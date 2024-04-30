using System.Collections.Generic;
using CraftingRPG.Enemies;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;

namespace CraftingRPG.Global;

public class EnemyInfo
{
    public static EnemyInfo Instance = new();

    private Dictionary<EnemyId, IEnemy> Enemies = new()
    {
        { EnemyId.GreenSlime, new GreenSlime() }
    };
    
    private EnemyInfo()
    {
    }

    public IEnemy GetEnemy(EnemyId enemyId)
    {
        return Enemies[enemyId];
    }
}