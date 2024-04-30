using System.Collections.Generic;
using CraftingRPG.Interfaces;

namespace CraftingRPG.QuestManagement;

public class QuestBook
{
    private IList<IQuestInstance> ActiveQuests { get; set; }

    public IList<IQuestInstance> GetActiveQuests() => ActiveQuests;
    public int GetActiveQuestCount() => ActiveQuests.Count;

    public QuestBook()
    {
        ActiveQuests = new List<IQuestInstance>();
    }
    
    public void AddQuest(IQuestInstance questInstance)
    {
        ActiveQuests.Add(questInstance);
    }
}