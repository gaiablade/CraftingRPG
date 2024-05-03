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
    public IDictionary<string, string> Properties { get; set; }
    
    public IList<TileLayer> TileLayers { get; set; }
    public IList<ObjectLayer> ObjectLayers { get; set; }
    public IList<MapTileSet> TileSets { get; set; }
    public IList<IEnemyInstance> Enemies { get; set; }
    public IList<LoadingZone> LoadingZones { get; set; }
}