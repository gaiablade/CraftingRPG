using System.Collections.Generic;
using CraftingRPG.Enums;

namespace CraftingRPG.Interfaces;

public interface IDefeatEnemyQuestInfo : IQuestInfo
{
    public Dictionary<EnemyId, int> GetRequiredEnemiesToDefeat();
}