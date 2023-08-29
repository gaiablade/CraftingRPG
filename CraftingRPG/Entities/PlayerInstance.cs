using CraftingRPG.Constants;
using CraftingRPG.Interfaces;
using Microsoft.Xna.Framework;

namespace CraftingRPG.Entities;

public class PlayerInstance : IInstance
{
    public PlayerInfo Info { get; set; }

    #region Movement
    public const float MovementSpeed = 3.5F;
    public Point Position = new Point();
    public int FacingDirection = Direction.Down;
    #endregion

    public PlayerInstance(PlayerInfo info)
    {
        Info = info;
    }

    public Point GetPosition() => Position;

    public int GetDepth() => Position.Y + 64;

    public Vector2 GetSize() => new Vector2(32, 64);

    public int GetSpriteSheetIndex() => SpriteIndex.Player1;
}
