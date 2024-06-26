﻿using CraftingRPG.Entities;
using CraftingRPG.Enums;
using System.Collections.Generic;

namespace CraftingRPG.Interfaces;

public interface IEnemy
{
    public EnemyId GetId();
    public string GetName();
    public List<EnemyDrop> GetDropTable();
    public int GetMaxHitPoints();
    public int GetAttackDamage();
}
