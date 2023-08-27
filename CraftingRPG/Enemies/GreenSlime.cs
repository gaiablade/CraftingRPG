using CraftingRPG.Entities;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using CraftingRPG.Items;
using CraftingRPG.Recipes;
using System.Collections.Generic;

namespace CraftingRPG.Enemies;

public class GreenSlime : IEnemy
{
    public List<EnemyDrop> GetDropTable() => new List<EnemyDrop>
    {
        new EnemyDrop(50, new ItemDrop<IronChunkItem>()),
        new EnemyDrop(20, new RecipeDrop<IronHelmetRecipe>())
    };

    public EnemyId GetId() => EnemyId.GreenSlime;

    public string GetName() => "Green Slime";

    public int GetSpriteSheetIndex() => 1;
}
