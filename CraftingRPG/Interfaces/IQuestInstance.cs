namespace CraftingRPG.Interfaces;

public interface IQuestInstance
{
    public IQuestInfo GetQuestInfo();
    public bool IsComplete();
}
