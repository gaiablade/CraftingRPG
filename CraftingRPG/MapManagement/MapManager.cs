using System;
using System.Collections.Generic;
using System.Linq;
using CraftingRPG.Constants;
using CraftingRPG.Enemies;
using CraftingRPG.Entities;
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
    private List<IDropInstance> DropsBelowPlayer = new();
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
        var camera = Global.Globals.Instance.Camera;

        // Step #1: Draw any layers below the player
        foreach (var tileLayer in CurrentMap.TileLayers)
        {
            this.DrawTileLayer(spriteBatch, tileLayer);
        }

        // Step #2: Draw any drops, enemies, player, etc.
        foreach (var objectLayer in CurrentMap.ObjectLayers)
        {
            foreach (var mapObject in objectLayer.Objects)
            {
                GameManager.SpriteBatch.Draw(mapObject.TileSet.SpriteSheetTexture,
                    new Rectangle(new Point((int)mapObject.X, (int)mapObject.Y),
                        new Point(mapObject.Width, mapObject.Height)),
                    mapObject.SourceRectangle,
                    Color.White);
            }
        }

        var instances = new List<IInstance>();
        instances.Add(Global.Globals.Instance.Player);
        instances.AddRange(CurrentMap.Enemies);
        instances.AddRange(Drops);
        instances.Sort((x, y) => x.GetDepth().CompareTo(y.GetDepth()));

        foreach (var instance in instances)
        {
            if (instance is PlayerInstance playerInstance)
            {
                DrawPlayer(playerInstance);
            }
            else if (instance is IEnemyInstance)
            {
                if (instance is EnemyInstance<GreenSlime> slimeInstance)
                {
                    GameManager.SpriteBatch.Draw(GameManager.SlimeSpriteSheet,
                        new Rectangle(slimeInstance.Position.ToPoint(), slimeInstance.GetSize().ToPoint()),
                        new Rectangle(0, 0, 32, 32),
                        Color.White);
                }
            }
            else
            {
                var size = instance.GetSize();
                var pos = instance.GetPosition();
                GameManager.SpriteBatch.Draw(GameManager.SpriteSheet,
                    new Rectangle(pos.ToPoint(), size.ToPoint()),
                    new Rectangle(0, instance.GetSpriteSheetIndex() * 32, (int)size.X, (int)size.Y),
                    Color.White);
            }
        }

        // Step #3: Draw any layers above the player

        // Step #4 Draw any UI
        if (DropsBelowPlayer.Count > 0)
        {
            GameManager.SpriteBatch.Draw(GameManager.Pixel,
                new Rectangle(new Point((int)camera.BoundingRectangle.Left, (int)camera.BoundingRectangle.Top),
                    new Point((int)camera.BoundingRectangle.Width + 10, 50)),
                new Color(0, 0, 0, 150));
            var dropName = DropsBelowPlayer.First().GetDroppable().GetName();
            var dropNameSize = Globals.Instance.Fnt15.MeasureString(dropName);
            GameManager.SpriteBatch.DrawString(Globals.Instance.Fnt15,
                dropName,
                new Vector2(camera.Center.X - dropNameSize.X / 2, camera.BoundingRectangle.Top),
                Color.White);
        }
    }

    public void Update(GameTime gameTime)
    {
        DetectAndHandleInput();
        DetectAndHandleMovement();
        DetectAndHandleCollisions();
        DetectAndHandleDropPickupEvent();
        DetectAndHandleAttack();

        UpdateDrops();
        CalculateCameraPosition();
        Global.Globals.Instance.Player.IsAboveDrop = IsPlayerAboveDropInstance(out DropsBelowPlayer);
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
        if (!player.IsAttacking)
        {
            Rectangle sourceRectangle;
            var flip = false;
            var spriteSize = GameManager.PlayerSpriteSize;
            var animFrame = player.IdleOrWalkingAnimFrames / 8 % 6;
            switch (player.FacingDirection)
            {
                case Direction.Up:
                    sourceRectangle = new Rectangle(new Point(0, spriteSize.Y * 2), spriteSize);
                    break;
                case Direction.Down:
                    sourceRectangle = new Rectangle(new Point(0, 0), spriteSize);
                    break;
                case Direction.Left:
                    sourceRectangle = new Rectangle(new Point(0, spriteSize.Y), spriteSize);
                    flip = true;
                    break;
                default: // Direction.Right
                    sourceRectangle = new Rectangle(new Point(0, spriteSize.Y), spriteSize);
                    break;
            }

            sourceRectangle.X = animFrame * spriteSize.X;
            if (Global.Globals.Instance.Player.IsWalking)
            {
                sourceRectangle.Y += spriteSize.Y * 3;
            }

            GameManager.SpriteBatch.Draw(Globals.Instance.PlayerSpriteSheet,
                new Rectangle(player.Position.ToPoint(), player.Size.ToPoint()),
                sourceRectangle,
                Color.White,
                0F,
                Vector2.Zero,
                flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                0);
        }
        else
        {
            Rectangle sourceRectangle;
            var flip = false;
            var spriteSize = GameManager.PlayerSpriteSize;
            var animFrame = player.AttackAnimFrames / PlayerInstance.AttackFrameLength;
            switch (player.FacingDirection)
            {
                case Direction.Up:
                    sourceRectangle = new Rectangle(new Point(0, spriteSize.Y * 8), spriteSize);
                    break;
                case Direction.Down:
                    sourceRectangle = new Rectangle(new Point(0, spriteSize.Y * 6), spriteSize);
                    break;
                case Direction.Left:
                    sourceRectangle = new Rectangle(new Point(0, spriteSize.Y * 7), spriteSize);
                    flip = true;
                    break;
                default: // Direction.Right
                    sourceRectangle = new Rectangle(new Point(0, spriteSize.Y * 7), spriteSize);
                    break;
            }

            sourceRectangle.X = animFrame * spriteSize.X;
            GameManager.SpriteBatch.Draw(Globals.Instance.PlayerSpriteSheet,
                new Rectangle(player.Position.ToPoint(), player.Size.ToPoint()),
                sourceRectangle,
                Color.White,
                0F,
                Vector2.Zero,
                flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                0);
        }
    }

    private void DetectAndHandleInput()
    {
        Global.Globals.Instance.ActionKeyPressed = GameManager.FramesKeysHeld[Keys.Z] == 1;
    }

    private void DetectAndHandleMovement()
    {
        var player = Global.Globals.Instance.Player;

        if (player.IsAttacking)
        {
            return;
        }

        player.MovementVector = Vector2.Zero;
        if (GameManager.FramesKeysHeld[Keys.Right] > 0)
        {
            player.MovementVector.X++;
        }
        else if (GameManager.FramesKeysHeld[Keys.Left] > 0)
        {
            player.MovementVector.X--;
        }

        if (GameManager.FramesKeysHeld[Keys.Up] > 0)
        {
            player.MovementVector.Y--;
        }
        else if (GameManager.FramesKeysHeld[Keys.Down] > 0)
        {
            player.MovementVector.Y++;
        }

        if (player.MovementVector != Vector2.Zero)
        {
            player.MovementVector = CustomMath.UnitVector(player.MovementVector);
            if (!player.IsWalking)
            {
                player.IsWalking = true;
                player.IdleOrWalkingAnimFrames = 0;
            }
            else
            {
                player.IdleOrWalkingAnimFrames++;
            }

            // If player is attacking, lock their facing direction
            if (!player.IsAttacking)
            {
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
            }

            player.Position.X += player.MovementVector.X * PlayerInstance.MovementSpeed;
            player.Position.Y += player.MovementVector.Y * PlayerInstance.MovementSpeed;
        }
        else
        {
            if (player.IsWalking)
            {
                player.IsWalking = false;
                player.IdleOrWalkingAnimFrames = 0;
            }
            else
            {
                player.IdleOrWalkingAnimFrames++;
            }
        }
    }

    private void DetectAndHandleCollisions()
    {
        // TODO: eventually we would want to check for collisions on all objects that have moved on a given frame.
        var player = Global.Globals.Instance.Player;

        var otherInstances = new List<IInstance>();
        otherInstances.AddRange(CurrentMap.Enemies);
        //otherInstances.AddRange(MapObjects);

        // If player's movement vector is zero, default to pushing them right
        if (player.MovementVector == Vector2.Zero)
        {
            player.MovementVector = new Vector2(1, 0);
        }

        // Check collision with other instance, e.g. enemies and objects
        foreach (var instance in otherInstances)
        {
            var otherColBox = instance.GetCollisionBox();
            while (otherColBox.Intersects(player.GetCollisionBox()))
            {
                player.Position.X -= player.MovementVector.X;
                player.Position.Y -= player.MovementVector.Y;
            }
        }

        // Check collision with solid map tiles
        var tiles = GetTilesBelowPlayer();
        // foreach (var tile in tiles)
        // {
        //     if (CurrentMap.CollisionMap[tile.Y][tile.X] == 2)
        //     {
        //         var tileCollisionBox = new Rectangle(tile.X * 32, tile.Y * 32, 32, 32);
        //         while (tileCollisionBox.Intersects(Player.GetCollisionBox()))
        //         {
        //             player.Position.X -= MovementVector.X;
        //             player.Position.Y -= MovementVector.Y;
        //         }
        //     }
        // }

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
    }

    private void DetectAndHandleDropPickupEvent()
    {
        var player = Global.Globals.Instance.Player;
        if (Global.Globals.Instance.ActionKeyPressed && player.IsAboveDrop)
        {
            var drop = DropsBelowPlayer.First();
            drop.GetDroppable().OnObtain();
            var i = Drops.IndexOf(drop);
            DropsBelowPlayer.Remove(drop);
            Drops.Remove(drop);
            DropInitialPositions.RemoveAt(i);
            DropHoverTimers.RemoveAt(i);
            Global.Globals.Instance.ActionKeyPressed = false;
        }
    }

    private void DetectAndHandleAttack()
    {
        var player = Global.Globals.Instance.Player;
        if (player.IsAttacking)
        {
            player.AttackAnimFrames++;
            if (player.AttackAnimFrames / PlayerInstance.AttackFrameLength > 3)
            {
                player.IsAttacking = false;
                AttackedEnemies.Clear();
                return;
            }

            // Check if attack collides with any enemies
            CalculateAttackHitbox();

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
                            var ran = Random.Shared.Next() % 100;
                            if (ran < possibleDrop.DropRate)
                            {
                                if (!possibleDrop.Drop.CanDrop())
                                {
                                    continue;
                                }

                                var dropInstance = new DropInstance(possibleDrop.Drop, inst.GetPosition());
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
            if (Global.Globals.Instance.ActionKeyPressed)
            {
                player.IsAttacking = true;
                player.AttackAnimFrames = 0;
                CalculateAttackHitbox();
                Global.Globals.Instance.ActionKeyPressed = false;
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

    private void CalculateAttackHitbox()
    {
        var player = Global.Globals.Instance.Player;
        var pSize = player.GetSize().ToPoint();
        var pPos = player.Position.ToPoint();
        var animFrame = player.AttackAnimFrames / PlayerInstance.AttackFrameLength;
        if (player.FacingDirection == Direction.Left)
        {
            player.AttackRect = animFrame switch
            {
                0 => new Rectangle(new Point(0, 0), new Point(0, 0)),
                1 => new Rectangle(new Point(pPos.X + 6, pPos.Y + 33), new Point(13, 14)),
                2 => new Rectangle(0, 0, 0, 0),
                3 => new Rectangle(0, 0, 0, 0),
                _ => player.AttackRect
            };
        }
        else if (player.FacingDirection == Direction.Right)
        {
            player.AttackRect = animFrame switch
            {
                0 => new Rectangle(new Point(0, 0), new Point(0, 0)),
                1 => new Rectangle(new Point(pPos.X + 32, pPos.Y + 32), new Point(13, 14)),
                2 => new Rectangle(0, 0, 0, 0),
                3 => new Rectangle(0, 0, 0, 0),
                _ => player.AttackRect
            };
        }
        else if (player.FacingDirection == Direction.Down)
        {
            player.AttackRect = animFrame switch
            {
                0 => new Rectangle(new Point(0, 0), new Point(0, 0)),
                1 => new Rectangle(pPos.X + 17, pPos.Y + 37, 22, 11),
                2 => new Rectangle(0, 0, 0, 0),
                3 => new Rectangle(0, 0, 0, 0),
                _ => player.AttackRect
            };
        }
        else if (player.FacingDirection == Direction.Up)
        {
            player.AttackRect = animFrame switch
            {
                0 => new Rectangle(new Point(0, 0), new Point(0, 0)),
                1 => new Rectangle(pPos.X + 11, pPos.Y + 22, 22, 11),
                2 => new Rectangle(0, 0, 0, 0),
                3 => new Rectangle(0, 0, 0, 0),
                _ => player.AttackRect
            };
        }
    }

    private void CalculateCameraPosition()
    {
        var player = Global.Globals.Instance.Player;
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
        Global.Globals.Instance.Camera = camera;
    }

    private HashSet<Point> GetTilesBelowPlayer()
    {
        var tiles = new HashSet<Point>();
        var pcb = Global.Globals.Instance.Player.GetCollisionBox();

        tiles.Add(new(pcb.X / 32, pcb.Y / 32));
        tiles.Add(new((pcb.X + 32) / 32, pcb.Y / 32));
        tiles.Add(new(pcb.X / 32, (pcb.Y + 32) / 32));
        tiles.Add(new((pcb.X + 32) / 32, (pcb.Y + 32) / 32));

        return tiles;
    }

    private bool IsPlayerAboveDropInstance(out List<IDropInstance> drops)
    {
        drops = new List<IDropInstance>();

        var playerBounds = Global.Globals.Instance.Player.GetBounds();

        foreach (var dropInstance in Drops)
        {
            var pos = dropInstance.GetPosition();
            var dropBounds = new Rectangle((int)pos.X, (int)pos.Y, 32, 32);
            if (dropBounds.Intersects(playerBounds))
            {
                drops.Add(dropInstance);
            }
        }

        return drops.Count > 0;
    }

    #endregion
}