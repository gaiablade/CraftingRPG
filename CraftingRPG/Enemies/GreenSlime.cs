using CraftingRPG.Constants;
using CraftingRPG.Entities;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using CraftingRPG.Items;
using CraftingRPG.Recipes;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace CraftingRPG.Enemies;

public class GreenSlime : IEnemy
{
    public List<EnemyDrop> GetDropTable() => new List<EnemyDrop>
    {
        new EnemyDrop(80, new ItemDrop<HealingMushroomItem>()),
        new EnemyDrop(70, new ItemDrop<EmptyBottleItem>()),
        new EnemyDrop(100, new RecipeDrop<IronHelmetRecipe>())
    };

    public EnemyId GetId() => EnemyId.GreenSlime;

    public string GetName() => "Green Slime";

    public int GetSpriteSheetIndex() => SpriteIndex.GreenSlime;

    public Rectangle GetCollisionBox() => new(8, 11, 16, 13);

    public int GetMaxHitPoints() => 50;
}
