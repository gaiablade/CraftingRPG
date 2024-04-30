using System.Collections.Generic;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;

namespace CraftingRPG.QuestManagement;

public class DefeatEnemyQuestInstance : BaseQuestInstance
{
    public Dictionary<EnemyId, int> DefeatedEnemies = new();
    
    public DefeatEnemyQuestInstance(IDefeatEnemyQuestInfo defeatEnemyQuestInfo)
    {
        QuestInfo = defeatEnemyQuestInfo;

        foreach (var (enemyId, _) in defeatEnemyQuestInfo.GetRequiredEnemiesToDefeat())
        {
            DefeatedEnemies[enemyId] = 0;
        }
    }

    public IDefeatEnemyQuestInfo GetDefeatEnemyQuestInfo() => (IDefeatEnemyQuestInfo)QuestInfo;

    public void OnEnemyDefeated(IEnemy enemy, int quantity = 1)
    {
        if (GetDefeatEnemyQuestInfo().GetRequiredEnemiesToDefeat().ContainsKey(enemy.GetId()))
        {
            DefeatedEnemies[enemy.GetId()] += quantity;
        }
    }

    public int GetDefeatedEnemyCount(EnemyId enemyId) => DefeatedEnemies[enemyId];
}