using Microsoft.Xna.Framework.Graphics;

namespace CraftingRPG.MapManagement;

public class MapTileSet
{
    public string Name { get; set; }
    public int TileWidth { get; set; }
    public int TileHeight { get; set; }
    public int Width { get; set; }
    public Texture2D SpriteSheetTexture { get; set; }
}