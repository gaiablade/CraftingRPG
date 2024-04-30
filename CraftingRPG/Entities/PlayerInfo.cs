using CraftingRPG.Interfaces;
using CraftingRPG.Items;
using CraftingRPG.PlayerStatistics;
using CraftingRPG.Recipes;
using CraftingRPG.QuestManagement;
using CraftingRPG.QuestManagement.Quests;
using CraftingRPG.RecipeBookManagement;

namespace CraftingRPG.Entities;

public class PlayerInfo : IPlayerInfo
{
    public RecipeBook RecipeBook { get; private set; } = new();
    public Inventory Inventory { get; private set; } = new();
    public PlayerEquipment Equipment { get; private set; } = new();
    public QuestBook QuestBook { get; private set; } = new QuestBook();
    public Statistics Stats { get; private set; } = new Statistics();

    public PlayerInfo()
    {
        Equipment.Weapon = new IronSwordItem();
        Equipment.Helmet = new IronHelmetItem();

        var mushroomQuest = new FetchQuestInstance(new HealingMushroomFetchQuestInfo());
        var mushroomQuest2 = new FetchQuestInstance(new HealingMushroomFetchQuestInfo());
        var potionQuest = new CraftQuestInstance(new SmallHealthPotionCraftQuestInfo());
        var slimeQuest = new DefeatEnemyQuestInstance(new DefeatSlimesQuestInfo());
        QuestBook.AddQuest(mushroomQuest);
        QuestBook.AddQuest(mushroomQuest2);
        QuestBook.AddQuest(potionQuest);
        QuestBook.AddQuest(slimeQuest);

        RecipeBook.AddRecipe(new SmallHealthPotionRecipe());
    }
}
