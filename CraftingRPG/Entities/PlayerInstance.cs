﻿using System.Drawing;
using CraftingRPG.Constants;
using CraftingRPG.Interfaces;
using Microsoft.Xna.Framework;
using Point = Microsoft.Xna.Framework.Point;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace CraftingRPG.Entities;

public class PlayerInstance : IInstance
{
    public PlayerInfo Info { get; set; }

    #region Movement

    public const float MovementSpeed = 3.5F;
    public Vector2 Position = new Vector2();
    public int FacingDirection = Direction.Down;

    #endregion

    public PlayerInstance(PlayerInfo info)
    {
        Info = info;
    }

    public Vector2 GetPosition() => Position;

    public float GetDepth() => Position.Y + 48;

    public Vector2 GetSize() => new Vector2(48 * 2, 48 * 2);

    public Rectangle GetBounds() => new Rectangle((int)Position.X, (int)Position.Y, (int)GetSize().X, (int)GetSize().Y);

    public int GetSpriteSheetIndex() => SpriteIndex.Player1;

    //public Rectangle GetCollisionBox() => new Rectangle((int)Position.X, (int)Position.Y + 32 + 16, 32, 16);
    public Rectangle GetCollisionBox() =>
        new Rectangle(new Point((int)Position.X + 18, (int)Position.Y + 48), new Point(22, 22));

    public Vector2 SetPosition(Vector2 position) => Position = position;
}