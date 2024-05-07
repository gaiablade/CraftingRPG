using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CraftingRPG.Constants;
using CraftingRPG.Entities.EnemyInstances;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using CraftingRPG.MapManagement;
using CraftingRPG.MapObjects;
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
        var tiledObjectLayers = tmxMap.ObjectGroups.Where(x => x.Name != "Enemies" && x.Name != "Loading Zones");
        var tiledMapProperties = tmxMap.Properties;
        var tiledEnemyLayers = tmxMap.ObjectGroups.Where(x => x.Name == "Enemies");
        var tiledLoadingZoneLayers = tmxMap.ObjectGroups.Where(x => x.Name == "Loading Zones");

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
                if (tiledTile.Gid == 0) continue;
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
            var mapObjects = new List<IMapObject>();
            foreach (var tiledObject in tiledObjectLayer.Objects)
            {
                var idFound = tiledObject.Properties.TryGetValue("object_type", out var idString);
                if (!idFound) continue;

                var id = GetMapObjectId(idString);
                IMapObject mapObject = id switch
                {
                    MapObjectId.Sign01 => new InteractiveSign(int.Parse(tiledObject.Properties["message_id"])),
                    MapObjectId.Sign02 => new DemoSign(),
                    MapObjectId.Chest01 => new Chest(int.Parse(tiledObject.Properties["chest_id"])),
                    _ => new MapObject()
                };
                mapObject.SetId(id);

                if (mapObject.GetId() == MapObjectId.Spawn)
                {
                    gameMap.Spawn = new Point((int)tiledObject.X, (int)tiledObject.Y);
                }

                var tileSetNo = tiledObject.Tile != null
                    ? GetTileSetForGid(tiledObject.Tile.Gid, firstGlobalIdentifiers, tiledTileSets)
                    : 0;
                mapObject.SetTileSet(tileSetNo != 0 ? gameTileSets[tileSetNo] : null);
                mapObject.SetSize(new Point((int)tiledObject.Width, (int)tiledObject.Height));
                mapObject.SetPosition(new Vector2((float)tiledObject.X, (float)tiledObject.Y - mapObject.GetSize().Y));
                mapObject.SetTextureRectangle(tileSetNo != 0 ? GetTileSourceRectangle(mapObject.GetTileSet(),
                    tiledObject.Tile.Gid - tiledTileSets[tileSetNo].FirstGid) : Rectangle.Empty);
                mapObject.SetAttributes(MapObjectAttributes.GetObjectAttributes(mapObject.GetId()));
                mapObjects.Add(mapObject);
            }

            mapObjects = mapObjects
                .OrderBy(x => x.GetPosition().Y)
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

        var loadingZones = new List<LoadingZone>();
        foreach (var loadingZoneLayer in tiledLoadingZoneLayers)
        {
            foreach (var tiledLoadingZone in loadingZoneLayer.Objects)
            {
                var x = float.Parse(tiledLoadingZone.Properties["toX"]);
                var y = float.Parse(tiledLoadingZone.Properties["toY"]);
                var loadingZone = new LoadingZone
                {
                    ToMap = tiledLoadingZone.Properties["map"],
                    ToPosition = new Vector2(x, y),
                    Size = new Point((int)tiledLoadingZone.Width, (int)tiledLoadingZone.Height),
                    Position = new Point((int)tiledLoadingZone.X, (int)tiledLoadingZone.Y)
                };

                var moveOutFound = tiledLoadingZone.Properties.TryGetValue("moveOut", out var moveOut);
                var moveInFound = tiledLoadingZone.Properties.TryGetValue("moveIn", out var moveIn);
                if (moveOutFound)
                {
                    loadingZone.MoveOut = moveOut switch
                    {
                        "west" => Direction.Left,
                        "east" => Direction.Right,
                        "north" => Direction.Up,
                        "south" => Direction.Down,
                        _ => Direction.Up
                    };
                }

                if (moveInFound)
                {
                    loadingZone.MoveIn = moveIn switch
                    {
                        "west" => Direction.Left,
                        "east" => Direction.Right,
                        "north" => Direction.Up,
                        "south" => Direction.Down,
                        _ => Direction.Up
                    };
                }

                loadingZones.Add(loadingZone);
            }
        }

        gameMap.TileSets = gameTileSets;
        gameMap.TileLayers = gameTileLayers;
        gameMap.ObjectLayers = objectLayers;
        gameMap.Enemies = enemies;
        gameMap.LoadingZones = loadingZones;

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
            case "tree02":
                return MapObjectId.Tree02;
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
            case "sign02":
                return MapObjectId.Sign02;
            case "grave01":
                return MapObjectId.Grave01;
            case "stone01":
                return MapObjectId.Stone01;
            case "stone02":
                return MapObjectId.Stone02;
            case "pot01":
                return MapObjectId.Pot01;
            case "fence01":
                return MapObjectId.Fence01;
            case "spawn":
                return MapObjectId.Spawn;
            case "chest01":
                return MapObjectId.Chest01;
            case "collision":
                return MapObjectId.Collision;
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