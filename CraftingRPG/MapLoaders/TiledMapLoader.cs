using System.Collections.Generic;
using System.IO;
using System.Linq;
using CraftingRPG.Entities.EnemyInstances;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using CraftingRPG.MapManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TiledSharp;

namespace CraftingRPG.MapLoaders;

public class TiledMapLoader : IMapLoader
{
    public static readonly TiledMapLoader Instance = new();

    public GameMap LoadMapFromFile(string mapFile, ContentManager contentManager)
    {
        var gameMap = new GameMap();
        var tmxMap = new TmxMap(mapFile);

        var tiledTileSets = tmxMap.Tilesets;
        var tiledTileLayers = tmxMap.Layers;
        var tiledObjectLayers = tmxMap.ObjectGroups.Where(x => x.Name != "Enemies");
        var tiledMapProperties = tmxMap.Properties;
        var tiledEnemyLayers = tmxMap.ObjectGroups.Where(x => x.Name == "Enemies");

        var firstGlobalIdentifiers = tiledTileSets
            .Select(x => x.FirstGid)
            .OrderBy(x => x)
            .ToList();

        gameMap.Name = mapFile;
        gameMap.Width = tmxMap.Width;
        gameMap.Height = tmxMap.Height;
        gameMap.TileWidth = tmxMap.TileWidth;
        gameMap.TileHeight = tmxMap.TileHeight;
        gameMap.Properties = tiledMapProperties
            .ToDictionary(x => x.Key, y => y.Value);

        var gameTileSets = new List<MapTileSet>();
        foreach (var tiledTileSet in tiledTileSets)
        {
            var gameTileSet = new MapTileSet();
            gameTileSet.Name = tiledTileSet.Name;
            gameTileSet.TileWidth = tiledTileSet.TileWidth;
            gameTileSet.TileHeight = tiledTileSet.TileHeight;
            gameTileSet.Width = tiledTileSet.Columns ?? 1;
            gameTileSet.SpriteSheetTexture =
                contentManager.Load<Texture2D>(Path.Join("textures",
                    Path.GetFileNameWithoutExtension(tiledTileSet.Image.Source)));
            gameTileSets.Add(gameTileSet);
        }

        var gameTileLayers = new List<TileLayer>();
        foreach (var tiledTileLayer in tiledTileLayers)
        {
            var tiles = new List<Tile>();
            foreach (var tiledTile in tiledTileLayer.Tiles)
            {
                var tile = new Tile();
                var tileSetNo = GetTileSetForGid(tiledTile.Gid, firstGlobalIdentifiers, tiledTileSets);
                tile.X = tiledTile.X;
                tile.Y = tiledTile.Y;
                tile.TileSet = gameTileSets[tileSetNo];
                tile.SourceRectangle =
                    GetTileSourceRectangle(tile.TileSet, tiledTile.Gid - tiledTileSets[tileSetNo].FirstGid);
                tiles.Add(tile);
            }

            var gameTileLayer = new TileLayer();
            gameTileLayer.Tiles = tiles;
            gameTileLayers.Add(gameTileLayer);
        }

        var objectLayers = new List<ObjectLayer>();
        foreach (var tiledObjectLayer in tiledObjectLayers)
        {
            var mapObjects = new List<MapObject>();
            foreach (var tiledObject in tiledObjectLayer.Objects)
            {
                var mapObject = new MapObject();
                mapObject.Id = GetMapObjectId(tiledObject.Properties["object_type"]);
                var tileSetNo = GetTileSetForGid(tiledObject.Tile.Gid, firstGlobalIdentifiers, tiledTileSets);
                mapObject.TileSet = gameTileSets[tileSetNo];
                mapObject.Width = (int)tiledObject.Width;
                mapObject.Height = (int)tiledObject.Height;
                mapObject.X = tiledObject.X;
                mapObject.Y = tiledObject.Y - mapObject.Height;
                mapObject.SourceRectangle =
                    GetTileSourceRectangle(mapObject.TileSet, tiledObject.Tile.Gid - tiledTileSets[tileSetNo].FirstGid);
                mapObject.Attributes = MapObjectAttributes.GetObjectAttributes(mapObject.Id);
                mapObjects.Add(mapObject);
            }

            mapObjects = mapObjects
                .OrderBy(x => x.Y)
                .ToList();

            var objectLayer = new ObjectLayer();
            objectLayer.Objects = mapObjects;
            objectLayers.Add(objectLayer);
        }

        var enemies = new List<IEnemyInstance>();
        foreach (var tiledEnemyLayer in tiledEnemyLayers)
        {
            foreach (var tiledEnemy in tiledEnemyLayer.Objects)
            {
                var enemyId = GetEnemyId(tiledEnemy.Properties["enemy_type"]);
                switch (enemyId)
                {
                    case EnemyId.GreenSlime:
                        var greenSlimeInstance = new GreenSlimeInstance();
                        greenSlimeInstance.SetPosition(new Vector2((int)tiledEnemy.X, (int)tiledEnemy.Y));
                        enemies.Add(greenSlimeInstance);
                        break;
                }
            }
        }

        gameMap.TileSets = gameTileSets;
        gameMap.TileLayers = gameTileLayers;
        gameMap.ObjectLayers = objectLayers;
        gameMap.Enemies = enemies;

        return gameMap;
    }

    private int GetTileSetForGid(int gid, List<int> firstGids, TmxList<TmxTileset> tiledTileSets)
    {
        for (var i = 0; i < firstGids.Count; i++)
        {
            var firstGid = firstGids[i];
            if (firstGid <= gid && firstGid + tiledTileSets[i].TileCount > gid)
            {
                return i;
            }
        }

        return -1;
    }

    private MapObjectId GetMapObjectId(string objectType)
    {
        switch (objectType)
        {
            case "tree01":
                return MapObjectId.Tree01;
            case "crate01":
                return MapObjectId.Crate01;
            case "log01":
                return MapObjectId.Log01;
            case "log02":
                return MapObjectId.Log02;
            case "bench01":
                return MapObjectId.Bench01;
            case "sign01":
                return MapObjectId.Sign01;
            case "grave01":
                return MapObjectId.Grave01;
            case "stone01":
                return MapObjectId.Stone01;
            case "stone02":
                return MapObjectId.Stone02;
            case "pot01":
                return MapObjectId.Pot01;
        }

        return MapObjectId.Bench01;
    }

    private EnemyId GetEnemyId(string enemyType)
    {
        switch (enemyType)
        {
            case "slime01":
                return EnemyId.GreenSlime;
        }

        return EnemyId.None;
    }

    private Rectangle GetTileSourceRectangle(MapTileSet tileSet, int gid)
    {
        var cols = tileSet.Width;

        var x = gid % cols * tileSet.TileWidth;
        var y = gid / cols * tileSet.TileHeight;

        return new Rectangle(x, y, tileSet.TileWidth, tileSet.TileHeight);
    }
}