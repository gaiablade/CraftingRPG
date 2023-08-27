using CraftingRPG.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
