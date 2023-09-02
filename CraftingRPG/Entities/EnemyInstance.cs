using CraftingRPG.Interfaces;
using Microsoft.Xna.Framework;

namespace CraftingRPG.Entities;

public class EnemyInstance<T> : IEnemyInstance where T : IEnemy
{
    public T Enemy { get; set; }
    public Vector2 Position { get; set; }
    public int CurrentHitPoints { get; set; }

    public EnemyInstance(T instance, Vector2 position)
    {
        Enemy = instance;
        Position = position;
        CurrentHitPoints = Enemy.GetMaxHitPoints();
    }

    public IEnemy GetEnemy() => Enemy;
    public Vector2 GetPosition() => Position;

    public float GetDepth() => Position.Y + 32;

    public Vector2 GetSize() => new Vector2(32, 32);

    public int GetSpriteSheetIndex() => Enemy.GetSpriteSheetIndex();

    public Rectangle GetCollisionBox()
    {
        var localColBox = Enemy.GetCollisionBox();
        return new Rectangle((int)Position.X + localColBox.X, 
            (int)Position.Y + localColBox.Y, 
            localColBox.Width, localColBox.Height);
    }

    public int GetCurrentHitPoints() => CurrentHitPoints;

    public bool IncurDamage(int damage)
    {
        CurrentHitPoints -= damage;
        if (CurrentHitPoints > 0)
            return false;
        return true;
    }
}
