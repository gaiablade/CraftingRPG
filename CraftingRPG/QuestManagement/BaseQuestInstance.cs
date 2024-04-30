using CraftingRPG.Interfaces;

namespace CraftingRPG.QuestManagement;

public class BaseQuestInstance : IQuestInstance
{
    protected IQuestInfo QuestInfo { get; set; }
    protected bool QuestComplete { get; set; } = false;

    public virtual IQuestInfo GetQuestInfo() => QuestInfo;
    public virtual bool IsComplete() => QuestComplete;
}