using CraftingRPG.Interfaces;
using CraftingRPG.Items;
using CraftingRPG.PlayerStatistics;
using CraftingRPG.QuestManagement;
using CraftingRPG.QuestManagement.Quests;
using CraftingRPG.RecipeManagement;
using CraftingRPG.RecipeManagement.Recipes;

namespace CraftingRPG.Entities;

public class PlayerInfo : IPlayerInfo
{
    public RecipeBook RecipeBook { get; } = new();
    public Inventory Inventory { get; private set; } = new();
    public PlayerEquipment Equipment { get; } = new();
    public QuestBook QuestBook { get; } = new();
    public Statistics Stats { get; private set; } = new();

    public PlayerInfo()
    {
        Equipment.Weapon = new IronSwordItem();
        Equipment.Helmet = new IronHelmetItem();

        var mushroomQuest = new FetchQuestInstance(new HealingMushroomFetchQuestInfo());
        var potionQuest = new CraftQuestInstance(new SmallHealthPotionCraftQuestInfo());
        var slimeQuest = new DefeatEnemyQuestInstance(new DefeatSlimesQuestInfo());
        QuestBook.AddQuest(mushroomQuest);
        QuestBook.AddQuest(potionQuest);
        QuestBook.AddQuest(slimeQuest);

        RecipeBook.AddRecipe(new SmallHealthPotionRecipe());
    }
}
