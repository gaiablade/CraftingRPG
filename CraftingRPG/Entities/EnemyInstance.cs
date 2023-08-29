using CraftingRPG.Interfaces;
using Microsoft.Xna.Framework;

namespace CraftingRPG.Entities;

public class EnemyInstance<T> : IEnemyInstance where T : IEnemy
{
    public T Enemy { get; set; }
    public Point Position { get; set; }

    public EnemyInstance(T instance, Point position)
    {
        Enemy = instance;
        Position = position;
    }

    public IEnemy GetEnemy() => Enemy;
    public Point GetPosition() => Position;

    public int GetDepth() => Position.Y + 32;

    public Vector2 GetSize() => new Vector2(32, 32);

    public int GetSpriteSheetIndex() => Enemy.GetSpriteSheetIndex();
}
