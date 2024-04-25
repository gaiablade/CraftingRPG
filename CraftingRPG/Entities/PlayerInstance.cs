using System.Drawing;
using CraftingRPG.Constants;
using CraftingRPG.Interfaces;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Point = Microsoft.Xna.Framework.Point;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using RectangleF = MonoGame.Extended.RectangleF;

namespace CraftingRPG.Entities;

public class PlayerInstance : IInstance
{
    public PlayerInfo Info { get; set; }

    #region Movement

    public const float MovementSpeed = 2F;
    public const int AttackFrameLength = 5;
    public static readonly Point SpriteSize = new Point(48, 48);
    
    public Vector2 Position = new Vector2();
    public Vector2 Size = new Vector2(48, 48);
    public int FacingDirection { get; set; } = Direction.Down;
    public bool IsAttacking { get; set; } = false;
    public int IdleOrWalkingAnimFrames { get; set; } = 0;
    public int AttackAnimFrames { get; set; } = 0;
    public bool IsWalking { get; set; } = false;
    public Vector2 MovementVector;
    public bool IsAboveDrop = false;
    public RectangleF AttackRect;

    #endregion

    public PlayerInstance(PlayerInfo info)
    {
        Info = info;
    }

    public Vector2 GetPosition() => Position;

    public float GetDepth() => Position.Y + GetSize().Y;

    public Vector2 GetSize() => new Vector2(48, 48);

    public Rectangle GetBounds() => new Rectangle((int)Position.X, (int)Position.Y, (int)GetSize().X, (int)GetSize().Y);

    public int GetSpriteSheetIndex() => SpriteIndex.Player1;

    public RectangleF GetCollisionBox() =>
        new RectangleF(new Point2(Position.X + 18, Position.Y + 22), new Size2(13, 19));

    public Vector2 SetPosition(Vector2 position) => Position = position;
}