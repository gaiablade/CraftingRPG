using System.Collections.Generic;
using CraftingRPG.Interfaces;

namespace CraftingRPG.MapManagement;

public class GameMap
{
    public string Name { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int TileWidth { get; set; }
    public int TileHeight { get; set; }
    public Dictionary<string, string> Properties { get; set; }
    
    public List<TileLayer> TileLayers { get; set; }
    public List<ObjectLayer> ObjectLayers { get; set; }
    public List<MapTileSet> TileSets { get; set; }
    public List<IEnemyInstance> Enemies { get; set; }
}