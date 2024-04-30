using System.Collections.Generic;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;

namespace CraftingRPG.QuestManagement.Quests;

public class DefeatSlimesQuestInfo : IDefeatEnemyQuestInfo
{
    public string GetName() => "Defeat 2 Green Slimes";

    public string GetDescription() => "Defeat 2 Green Slimes to complete this quest";

    public Dictionary<EnemyId, int> GetRequiredEnemiesToDefeat() => new()
    {
        { EnemyId.GreenSlime, 2 }
    };
}