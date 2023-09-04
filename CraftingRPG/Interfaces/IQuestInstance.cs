namespace CraftingRPG.Interfaces;

public interface IQuestInstance
{
    public IQuest GetQuest();
    public bool IsComplete();
}
