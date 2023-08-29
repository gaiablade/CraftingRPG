using CraftingRPG.Interfaces;
using Microsoft.Xna.Framework;
using System;

namespace CraftingRPG.Entities;

public class MapObjectInstance<T> : IInstance where T : IMapObject, new()
{
    private T Instance;
    private Point Position;

    public MapObjectInstance(Point position)
    {
        Position = position;
        Instance = new();
    }

    public int GetDepth() => Position.Y + 32;

    public Point GetPosition() => Position;

    public Vector2 GetSize() => new Vector2(32, 32);

    public int GetSpriteSheetIndex() => Instance.GetSpriteSheetIndex();
}
