using CraftingRPG.Constants;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using System.Collections.Generic;

namespace CraftingRPG.Items;

public class ArcaneFlowerItem : IItem
{
    public ItemId GetId() => ItemId.ArcaneFlower;

    public ISet<ItemCategory> GetItemCategories() => new HashSet<ItemCategory>
    {
        ItemCategory.Ingredient
    };

    public string GetName() => "Arcane Flower";

    public int GetSpriteSheetIndex() => SpriteIndex.QuestionMark;
}
