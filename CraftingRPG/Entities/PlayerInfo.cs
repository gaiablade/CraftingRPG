using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using CraftingRPG.Items;
using CraftingRPG.Quests;
using System.Collections.Generic;

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

        var mushroomQuest = new FetchQuestInstance(new FetchQuestMushrooms());
        mushroomQuest.AddCollectedItem(ItemId.HealingMushroom, 9);
        mushroomQuest.AddCollectedItem(ItemId.EmptyBottle, 4);
        Quests.Add(mushroomQuest);
    }
}
