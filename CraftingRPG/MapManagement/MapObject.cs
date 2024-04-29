using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace CraftingRPG.MapManagement;

public class MapObject : IInstance
{
    public MapObjectId Id { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public MapTileSet TileSet { get; set; }
    public Rectangle SourceRectangle { get; set; }
    public MapObjectAttributes Attributes { get; set; }

    public int GetSpriteSheetIndex()
    {
        return -1;
    }

    public RectangleF GetCollisionBox()
    {
        var clBx = this.Attributes.CollisionRectangle;
        return new RectangleF((float)(clBx.X + X), (float)(clBx.Y + Y), clBx.Width, clBx.Height);
    }

    public Texture2D GetSpriteSheet()
    {
        return TileSet.SpriteSheetTexture;
    }

    public Rectangle GetTextureRectangle()
    {
        return SourceRectangle;
    }

    public Vector2 GetPosition()
    {
        return new Vector2((float)X, (float)Y);
    }

    public Vector2 SetPosition(Vector2 position)
    {
        return Vector2.Zero;
    }

    public double GetDepth()
    {
        return GetCollisionBox().Y + GetCollisionBox().Height;
    }

    public Point GetSize()
    {
        return new Point(Width, Height);
    }
}