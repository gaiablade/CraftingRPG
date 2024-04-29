using CraftingRPG.Interfaces;
using CraftingRPG.Items;
using CraftingRPG.Quests;
using CraftingRPG.Recipes;
using System.Collections.Generic;
using CraftingRPG.RecipeBookManagement;

namespace CraftingRPG.Entities;

public class PlayerInfo : IPlayerInfo
{
    public RecipeBook RecipeBook { get; private set; } = new();
    public Inventory Inventory { get; private set; } = new();
    public PlayerEquipment Equipment { get; private set; } = new();
    public List<IQuestInstance> Quests { get; private set; } = new();

    public PlayerInfo()
    {
        Equipment.Weapon = new IronSwordItem();
        Equipment.Helmet = new IronHelmetItem();

        var mushroomQuest = new FetchQuestInstance(new FetchQuestSmallPotionIngredients());
        Quests.Add(mushroomQuest);

        RecipeBook.AddRecipe(new SmallHealthPotionRecipe());
    }
}
