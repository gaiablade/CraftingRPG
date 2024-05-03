using System.Collections.Generic;
using CraftingRPG.Interfaces;

namespace CraftingRPG.QuestManagement;

public class QuestBook
{
    private IList<IQuestInstance> ActiveQuests { get; set; }
    private IList<FetchQuestInstance> FetchQuests { get; set; }
    private IList<DefeatEnemyQuestInstance> DefeatEnemyQuests { get; set; }
    private IList<CraftQuestInstance> CraftQuests { get; set; }

    public IList<IQuestInstance> GetActiveQuests() => ActiveQuests;
    public IList<FetchQuestInstance> GetFetchQuests() => FetchQuests;
    public IList<DefeatEnemyQuestInstance> GetDefeatEnemyQuests() => DefeatEnemyQuests;
    public IList<CraftQuestInstance> GetCraftQuests() => CraftQuests;
    public int GetActiveQuestCount() => ActiveQuests.Count;

    public QuestBook()
    {
        ActiveQuests = new List<IQuestInstance>();
        FetchQuests = new List<FetchQuestInstance>();
        DefeatEnemyQuests = new List<DefeatEnemyQuestInstance>();
        CraftQuests = new List<CraftQuestInstance>();
    }
    
    public void AddQuest(IQuestInstance questInstance)
    {
        ActiveQuests.Add(questInstance);
        switch (questInstance)
        {
            case FetchQuestInstance fetchQuestInstance:
                FetchQuests.Add(fetchQuestInstance);
                break;
            case DefeatEnemyQuestInstance defeatEnemyQuestInstance:
                DefeatEnemyQuests.Add(defeatEnemyQuestInstance);
                break;
            case CraftQuestInstance craftQuestInstance:
                CraftQuests.Add(craftQuestInstance);
                break;
        }
    }
}