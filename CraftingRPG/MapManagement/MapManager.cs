using System.Collections.Generic;
using System.IO;
using System.Linq;
using CraftingRPG.Enemies;
using CraftingRPG.Entities;
using CraftingRPG.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using TiledSharp;

namespace CraftingRPG.MapManagement;

public class MapManager
{
    public static readonly MapManager Instance = new();

    private readonly string[] MapsToLoad =
    {
        "Tmx/map1.tmx"
    };

    private readonly Dictionary<string, TmxMap> Maps = new();
    private readonly Dictionary<string, TmxTileset> TileSets = new();
    private readonly Dictionary<string, Texture2D> TileSetTextures = new();
    public List<IEnemyInstance> Enemies = new();
    public TmxMap GetMap(string key) => Maps[key];
    public Texture2D GetTileSetTexture(string key) => TileSetTextures[key];
    public TmxMap CurrentMap { get; set; }
    public TmxList<TmxTileset> CurrentTileSets { get; set; }
    public List<int> FirstGlobalIdentifiers { get; set; }

    public void LoadMapsFromContents(ContentManager contentManager)
    {
        foreach (var mapToLoad in MapsToLoad)
        {
            var map = new TmxMap(mapToLoad);
            Maps[mapToLoad] = map;
            foreach (var tileSet in map.Tilesets)
            {
                TileSets.Add(tileSet.Name, tileSet);
                if (string.IsNullOrEmpty(tileSet.Image.Source)) continue;
                var source = Path.Join("textures", Path.GetFileNameWithoutExtension(tileSet.Image.Source));
                TileSetTextures.Add(tileSet.Image.Source, contentManager.Load<Texture2D>(source));
            }
        }
    }

    public void LoadDefaultMap()
    {
        SetMap("Tmx/map1.tmx");
    }

    public void SetMap(string mapName)
    {
        CurrentMap = GetMap(mapName);
        CurrentTileSets = CurrentMap.Tilesets;
        FirstGlobalIdentifiers = CurrentTileSets
            .Select(x => x.FirstGid)
            .OrderBy(x => x)
            .ToList();
        Enemies.Clear();
        var enemyLayer = CurrentMap.ObjectGroups
            .FirstOrDefault(x => x.Name == "Enemies")?.Objects;

        if (enemyLayer != null)
        {
            foreach (var enemy in enemyLayer)
            {
                switch (enemy.Properties["enemy_type"])
                {
                    case "slime01":
                        Enemies.Add(new EnemyInstance<GreenSlime>(new GreenSlime(),
                            new Vector2((float)enemy.X, (float)enemy.Y)));
                        break;
                }
            }
        }
    }

    public void DrawMap(SpriteBatch spriteBatch)
    {
        foreach (var layer in CurrentMap.Layers)
        {
            foreach (var tile in layer.Tiles)
            {
                if (tile.Gid == 0) continue;
                var pos = new Point(tile.X, tile.Y);
                var gid = tile.Gid;
                var tileSetNo = GetTileSetForGid(gid);
                var tileSet = CurrentTileSets[tileSetNo];
                var tileSetTex = TileSetTextures[tileSet.Image.Source];
                var tileId = gid - tileSet.FirstGid;
                var gridPos = GetTileSourceRectangle(tileSet, tileId);
                var tWidth = tileSet.TileWidth;
                var tHeight = tileSet.TileHeight;

                spriteBatch.Draw(texture: tileSetTex,
                    destinationRectangle: new Rectangle(pos.X * tWidth, pos.Y * tHeight, tWidth, tHeight),
                    sourceRectangle: gridPos,
                    color: Color.White);
            }
        }

        foreach (var layer in CurrentMap.ObjectGroups.Where(x => x.Name != "Enemies"))
        {
            foreach (var obj in layer.Objects.OrderBy(x => x.Y))
            {
                var pos = new Point((int)obj.X, (int)obj.Y);
                var tileSetNo = GetTileSetForGid(obj.Tile.Gid);
                var tileSet = CurrentTileSets[tileSetNo];
                var sourceRect = GetTileSourceRectangle(tileSet, obj.Tile.Gid - tileSet.FirstGid);
                var tex = TileSetTextures[tileSet.Image.Source];

                GameManager.SpriteBatch.Draw(tex,
                    destinationRectangle: new Rectangle(pos.X, pos.Y - tileSet.TileHeight, tileSet.TileWidth,
                        tileSet.TileHeight),
                    sourceRectangle: sourceRect,
                    color: Color.White);
            }
        }
    }

    private Rectangle GetTileSourceRectangle(TmxTileset tileSet, int gid)
    {
        var cols = tileSet.Columns ?? 0;

        var x = gid % cols * tileSet.TileWidth;
        var y = gid / cols * tileSet.TileHeight;

        return new Rectangle(x, y, tileSet.TileWidth, tileSet.TileHeight);
    }

    private int GetTileSetForGid(int gid)
    {
        for (var i = 0; i < FirstGlobalIdentifiers.Count; i++)
        {
            var firstGid = FirstGlobalIdentifiers[i];
            if (firstGid <= gid && firstGid + CurrentTileSets[i].TileCount > gid)
            {
                return i;
            }
        }

        return -1;
    }
}