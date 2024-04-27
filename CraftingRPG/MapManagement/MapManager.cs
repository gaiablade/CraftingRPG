using System;
using System.Collections.Generic;
using System.Linq;
using CraftingRPG.Constants;
using CraftingRPG.Entities;
using CraftingRPG.Extensions;
using CraftingRPG.Global;
using CraftingRPG.Interfaces;
using CraftingRPG.MapLoaders;
using CraftingRPG.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using TiledSharp;

namespace CraftingRPG.MapManagement;

public class MapManager
{
    public static readonly MapManager Instance = new();

    private Dictionary<string, GameMap> Maps = new();
    private GameMap CurrentMap;

    private readonly string[] MapsToLoad =
    {
        "Tmx/map1.tmx"
    };

    private List<IDropInstance> Drops = new();
    private List<Vector2> DropInitialPositions = new();
    private List<int> DropHoverTimers = new();
    private List<IEnemyInstance> AttackedEnemies = new();

    public GameMap GetMap(string key) => Maps[key];
    public GameMap GetCurrentMap() => CurrentMap;

    #region Public Methods

    public void LoadMapsFromContents(ContentManager contentManager)
    {
        foreach (var mapToLoad in MapsToLoad)
        {
            var map = TiledMapLoader.Instance.LoadMapFromFile(mapToLoad, contentManager);
            Maps[mapToLoad] = map;
        }
    }

    public void LoadDefaultMap()
    {
        SetMap("Tmx/map1.tmx");
    }

    public void SetMap(string mapName)
    {
        CurrentMap = GetMap(mapName);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var player = Globals.Instance.Player;
        var camera = Globals.Instance.Camera;

        foreach (var tileLayer in CurrentMap.TileLayers)
        {
            DrawTileLayer(spriteBatch, tileLayer);
        }

        var instances = new List<IInstance>();
        instances.Add(Globals.Instance.Player);
        instances.AddRange(CurrentMap.Enemies);
        foreach (var objectLayer in CurrentMap.ObjectLayers)
        {
            instances.AddRange(objectLayer.Objects);
        }

        instances.AddRange(Drops);

        // TODO: Remove Linq
        instances = instances.OrderBy(x => x.GetDepth()).ThenBy(x => x.GetPosition().X).ToList();

        foreach (var instance in instances)
        {
            if (instance is PlayerInstance playerInstance)
            {
                DrawPlayer(playerInstance);
            }
            else if (instance is IEnemyInstance enemyInstance)
            {
                GameManager.SpriteBatch.Draw(enemyInstance.GetSpriteSheet(),
                    new Rectangle(enemyInstance.GetPosition().ToPoint(), enemyInstance.GetSize().ToPoint()),
                    enemyInstance.GetTextureRectangle(),
                    Color.White);
            }
            else if (instance is MapObject mapObject)
            {
                GameManager.SpriteBatch.Draw(mapObject.TileSet.SpriteSheetTexture,
                    new Rectangle(new Point((int)mapObject.X, (int)mapObject.Y),
                        new Point(mapObject.Width, mapObject.Height)),
                    mapObject.SourceRectangle,
                    Color.White);
            }
            else if (instance is IDropInstance dropInstance)
            {
                GameManager.SpriteBatch.Draw(dropInstance.GetSpriteSheet(),
                    new Rectangle(dropInstance.GetPosition().ToPoint(), dropInstance.GetSize().ToPoint()),
                    dropInstance.GetTextureRectangle(),
                    Color.White);
            }
        }
    }

    public void Update(GameTime gameTime)
    {
        DetectAndHandleInput();
        DetectAndHandleMovement(gameTime);
        DetectAndHandleCollisions();
        DetectAndHandleDropPickupEvent();
        DetectAndHandleAttack();

        Globals.Instance.Player.UpdateAnimation(gameTime);
        foreach (var enemy in CurrentMap.Enemies)
        {
        }

        UpdateDrops();
        CalculateCameraPosition();
        Globals.Instance.Player.IsAboveDrop = IsPlayerAboveDropInstance(out var dropsBelowPlayer);
        Globals.Instance.Player.DropsBelowPlayer = dropsBelowPlayer;
    }

    public void AddDrop(IDropInstance drop)
    {
        Drops.Add(drop);
    }

    #endregion

    #region Private Methods

    private Rectangle GetTileSourceRectangle(TmxTileset tileSet, int gid)
    {
        var cols = tileSet.Columns ?? 0;

        var x = gid % cols * tileSet.TileWidth;
        var y = gid / cols * tileSet.TileHeight;

        return new Rectangle(x, y, tileSet.TileWidth, tileSet.TileHeight);
    }

    private void DrawTileLayer(SpriteBatch spriteBatch, TileLayer tileLayer)
    {
        foreach (var tile in tileLayer.Tiles)
        {
            spriteBatch.Draw(texture: tile.TileSet.SpriteSheetTexture,
                destinationRectangle: new Rectangle(
                    new Point(tile.X * tile.TileSet.TileWidth, tile.Y * tile.TileSet.TileHeight),
                    new Point(tile.TileSet.TileWidth, tile.TileSet.TileHeight)),
                sourceRectangle: tile.SourceRectangle,
                color: Color.White);
        }
    }

    private void DrawPlayer(PlayerInstance player)
    {
        var flip = player.FacingDirection == Direction.Left;
        GameManager.SpriteBatch.Draw(player.GetSpriteSheet(),
            new Rectangle(player.Position.ToPoint(), PlayerInstance.SpriteSize),
            player.GetTextureRectangle(),
            Color.White,
            0F,
            Vector2.Zero,
            flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
            0);
    }

    private void DetectAndHandleInput()
    {
        Globals.Instance.ActionKeyPressed = GameManager.FramesKeysHeld[Keys.Z] == 1;
    }

    private void DetectAndHandleMovement(GameTime gameTime)
    {
        var player = Globals.Instance.Player;

        player.MovementVector = Vector2.Zero;

        if (player.IsAttacking)
        {
            return;
        }

        if (GameManager.FramesKeysHeld[Keys.Right] > 0)
        {
            player.MovementVector.X = 1;
        }
        else if (GameManager.FramesKeysHeld[Keys.Left] > 0)
        {
            player.MovementVector.X = -1;
        }

        if (GameManager.FramesKeysHeld[Keys.Up] > 0)
        {
            player.MovementVector.Y = -1;
        }
        else if (GameManager.FramesKeysHeld[Keys.Down] > 0)
        {
            player.MovementVector.Y = 1;
        }

        if (player.MovementVector != Vector2.Zero)
        {
            player.MovementVector = CustomMath.UnitVector(player.MovementVector);

            // Animation
            if (!player.IsWalking)
            {
                player.IsWalking = true;
            }

            // If player is attacking, lock their facing direction
            if (player.MovementVector.Y > 0)
            {
                player.FacingDirection = Direction.Down;
            }
            else if (player.MovementVector.Y < 0)
            {
                player.FacingDirection = Direction.Up;
            }
            else if (player.MovementVector.X > 0)
            {
                player.FacingDirection = Direction.Right;
            }
            else if (player.MovementVector.X < 0)
            {
                player.FacingDirection = Direction.Left;
            }

            player.MovementVector = Vector2.Multiply(player.MovementVector, PlayerInstance.MovementSpeed);
        }
        else
        {
            if (player.IsWalking)
            {
                player.IsWalking = false;
            }
        }
    }

    private void DetectAndHandleCollisions()
    {
        // TODO: eventually we would want to check for collisions on all objects that have moved on a given frame.
        var player = Globals.Instance.Player;

        var projectedPlayerBounds = player.GetCollisionBox();
        projectedPlayerBounds.X += player.MovementVector.X;
        projectedPlayerBounds.Y += player.MovementVector.Y;

        // Check for collision with map objects
        foreach (var objectLayer in CurrentMap.ObjectLayers)
        {
            foreach (var mapObject in objectLayer.Objects)
            {
                var attr = mapObject.Attributes;
                var objCBox = mapObject.GetCollisionBox();

                if (!attr.IsSolid || !projectedPlayerBounds.Intersects(objCBox)) continue;

                var depth = projectedPlayerBounds.GetIntersectionDepth(objCBox);
                var absDepth = new Vector2(Math.Abs(depth.X), Math.Abs(depth.Y));

                if (absDepth.Y < absDepth.X)
                {
                    player.MovementVector.Y += depth.Y;
                }
                else
                {
                    player.MovementVector.X += depth.X;
                }
            }
        }

        // Check if any drops are colliding
        for (var i = 0; i < Drops.Count - 1; i++)
        {
            var cb1 = Drops[i].GetCollisionBox();
            for (int j = i + 1; j < Drops.Count; j++)
            {
                var cb2 = Drops[j].GetCollisionBox();
                if (cb2.Intersects(cb1))
                {
                    var pos1 = Drops[i].GetPosition();
                    var pos2 = Drops[j].GetPosition();
                    Drops[i].SetPosition(new Vector2(pos1.X - 1, pos1.Y));
                    Drops[j].SetPosition(new Vector2(pos2.X + 1, pos2.Y));
                }
            }
        }

        // Do movement
        player.Position.X += player.MovementVector.X;
        player.Position.Y += player.MovementVector.Y;
    }

    private void DetectAndHandleDropPickupEvent()
    {
        var player = Globals.Instance.Player;
        if (Globals.Instance.ActionKeyPressed && player.IsAboveDrop)
        {
            var drop = player.DropsBelowPlayer.First();
            drop.OnObtain();
            var i = Drops.IndexOf(drop);
            player.DropsBelowPlayer.Remove(drop);
            Drops.Remove(drop);
            DropInitialPositions.RemoveAt(i);
            DropHoverTimers.RemoveAt(i);
            Globals.Instance.ActionKeyPressed = false;
        }
    }

    private void DetectAndHandleAttack()
    {
        var player = Globals.Instance.Player;
        if (player.IsAttacking)
        {
            foreach (var inst in CurrentMap.Enemies)
            {
                if (!AttackedEnemies.Contains(inst) && inst.GetCollisionBox().Intersects(player.AttackRect))
                {
                    GameManager.HitSfx01.Play(0.3F, 0F, 0F);
                    var damage = player.Info.Equipment.Weapon.GetAttackStat();
                    var isDefeated = inst.IncurDamage(damage);
                    if (!isDefeated)
                    {
                        AttackedEnemies.Add(inst);
                    }
                    else
                    {
                        var dropTable = inst.GetEnemy().GetDropTable();
                        foreach (var possibleDrop in dropTable)
                        {
                            var randomNumber = Random.Shared.Next() % 100;
                            if (randomNumber < possibleDrop.DropRate)
                            {
                                var dropInstance = possibleDrop.CreateDropInstance();

                                if (!dropInstance.CanDrop())
                                {
                                    continue;
                                }

                                dropInstance.SetPosition(inst.GetPosition());

                                Drops.Add(dropInstance);
                                DropHoverTimers.Add(0);
                                DropInitialPositions.Add(dropInstance.GetPosition());
                            }
                        }
                    }
                }
            }

            CurrentMap.Enemies.RemoveAll(x => x.GetCurrentHitPoints() <= 0);
        }
        else
        {
            AttackedEnemies.Clear();
            if (Globals.Instance.ActionKeyPressed)
            {
                player.IsAttacking = true;
                Globals.Instance.ActionKeyPressed = false;
                GameManager.SwingSfx01.Play(volume: 0.1F, 0F, 0F);
            }
        }
    }

    private void UpdateDrops()
    {
        for (var i = 0; i < Drops.Count; i++)
        {
            DropHoverTimers[i]++;
            var iPos = DropInitialPositions[i];
            var pos = Drops[i].GetPosition();
            Drops[i].SetPosition(new Vector2(pos.X, iPos.Y + 10 * (float)Math.Sin((double)DropHoverTimers[i] / 50)));
        }
    }

    private void CalculateCameraPosition()
    {
        var player = Globals.Instance.Player;
        var camera = new OrthographicCamera(GameManager.SpriteBatch.GraphicsDevice);
        camera.Zoom = 2F;
        var x = camera.BoundingRectangle;
        var mapWidth = CurrentMap.Width * CurrentMap.TileWidth;
        var mapHeight = CurrentMap.Height * CurrentMap.TileHeight;

        var cameraPos = Vector2.Zero;
        // Center the player
        cameraPos.X = player.Position.X + player.GetSize().X / 2;
        cameraPos.Y = player.Position.Y + player.GetSize().Y / 2;

        if (cameraPos.X - x.Width / 2 < 0)
        {
            cameraPos.X = x.Width / 2;
        }
        else if (cameraPos.X + x.Width / 2 > mapWidth)
        {
            cameraPos.X = mapWidth - x.Width / 2;
        }

        if (cameraPos.Y - x.Height / 2F < 0)
        {
            cameraPos.Y = x.Height / 2F;
        }
        else if (cameraPos.Y + x.Height / 2F > mapHeight)
        {
            cameraPos.Y = mapHeight - x.Height / 2F;
        }

        camera.LookAt(cameraPos);
        Globals.Instance.Camera = camera;
    }

    private HashSet<Point> GetTilesBelowPlayer()
    {
        var tiles = new HashSet<Point>();
        var pcb = Globals.Instance.Player.GetCollisionBox().ToRectangle();

        tiles.Add(new(pcb.X / 32, pcb.Y / 32));
        tiles.Add(new((pcb.X + 32) / 32, pcb.Y / 32));
        tiles.Add(new(pcb.X / 32, (pcb.Y + 32) / 32));
        tiles.Add(new((pcb.X + 32) / 32, (pcb.Y + 32) / 32));

        return tiles;
    }

    private bool IsPlayerAboveDropInstance(out List<IDropInstance> drops)
    {
        drops = new List<IDropInstance>();

        var playerBounds = Globals.Instance.Player.GetBounds();

        foreach (var dropInstance in Drops)
        {
            var pos = dropInstance.GetPosition();
            var dropBounds = new Rectangle((int)pos.X, (int)pos.Y, 16, 16);
            if (dropBounds.Intersects(playerBounds))
            {
                drops.Add(dropInstance);
            }
        }

        return drops.Count > 0;
    }

    #endregion
}