using Microsoft.Xna.Framework;

namespace CraftingRPG.MapManagement;

public class Tile
{
    public MapTileSet TileSet { get; set; }
    public Rectangle SourceRectangle { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
}