using CraftingRPG.Entities;
using CraftingRPG.Enums;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace CraftingRPG.Interfaces;

public interface IEnemy
{
    public EnemyId GetId();
    public string GetName();
    public List<EnemyDrop> GetDropTable();
    public int GetSpriteSheetIndex();
    public Rectangle GetCollisionBox();
    public int GetMaxHitPoints();
}
