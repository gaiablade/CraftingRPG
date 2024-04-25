using CraftingRPG.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CraftingRPG.MapManagement;

public class MapObject
{
    public MapObjectId Id { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public MapTileSet TileSet { get; set; }
    public Rectangle SourceRectangle { get; set; }
    public MapObjectAttributes Attributes { get; set; }

    public Rectangle GetCollisionBox()
    {
        var clBx = this.Attributes.CollisionRectangle;
        return new Rectangle((int)(clBx.X + X), (int)(clBx.Y + Y), clBx.Width, clBx.Height);
    }
}