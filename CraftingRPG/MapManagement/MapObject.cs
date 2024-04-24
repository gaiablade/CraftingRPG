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
}