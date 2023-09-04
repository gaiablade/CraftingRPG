﻿using CraftingRPG.Constants;
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
        new EnemyDrop(30, new ItemDrop<HealingMushroomItem>()),
        new EnemyDrop(10, new RecipeDrop<SmallHealthPotionRecipe>()),
        new EnemyDrop(20, new ItemDrop<EmptyBottleItem>())
    };

    public EnemyId GetId() => EnemyId.GreenSlime;

    public string GetName() => "Green Slime";

    public int GetSpriteSheetIndex() => SpriteIndex.GreenSlime;

    public Rectangle GetCollisionBox() => new(0, 16, 32, 16);

    public int GetMaxHitPoints() => 50;
}
