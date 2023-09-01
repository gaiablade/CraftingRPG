﻿using CraftingRPG.Interfaces;
using Microsoft.Xna.Framework;
using System;

namespace CraftingRPG.Entities;

public class MapObjectInstance<T> : IInstance where T : IMapObject, new()
{
    private T Instance;
    private Vector2 Position;

    public MapObjectInstance(Vector2 position)
    {
        Position = position;
        Instance = new();
    }

    public Rectangle GetCollisionBox()
    {
        var locColBox = Instance.GetCollisionBox();
        return new Rectangle((int)Position.X + locColBox.X, (int)Position.Y + locColBox.Y,
            locColBox.Width, locColBox.Height);
    }

    public float GetDepth() => Position.Y + 32;

    public Vector2 GetPosition() => Position;

    public Vector2 GetSize() => new Vector2(32, 32);

    public int GetSpriteSheetIndex() => Instance.GetSpriteSheetIndex();
}
