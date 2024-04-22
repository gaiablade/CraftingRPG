using CraftingRPG.Constants;
using CraftingRPG.Enemies;
using CraftingRPG.Entities;
using CraftingRPG.Interfaces;
using CraftingRPG.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using CraftingRPG.Enums;
using CraftingRPG.MapManagement;
using TiledSharp;

// I love you

namespace CraftingRPG.States;

public class OverworldState : IState
{
    private PlayerInstance Player;
    private List<IInstance> MapObjects;
    private List<IDropInstance> Drops;
    private List<int> DropHoverTimers;
    private List<Vector2> DropInitialPositions;
    private Vector2 MovementVector;
    private bool IsAttacking = false;
    private int AttackAnimFrames = 0;
    private int AttackFrameLength = 8;
    private bool IsWalking = false;
    private int IdleOrWalkingAnimFrames = 0;
    private int AttackFrame = 0;
    private Rectangle AttackRect;
    private List<IEnemyInstance> AttackedEnemies = new();
    private bool IsAboveDrop = false;
    private List<IDropInstance> DropsBelowPlayer;
    private bool ActionKeyPressed = false;

    private OverworldMap CurrentMap;

    private TmxMap Map;
    private int TileWidth;
    private int TileHeight;

    public OverworldState()
    {
        GameManager.AddKeysIfNotExists(Keys.Left, Keys.Right, Keys.Up, Keys.Down, Keys.Z, Keys.X, Keys.C, Keys.I,
            Keys.Q);

        Player = new PlayerInstance(GameManager.PlayerInfo);
        Player.Position = new Vector2(100, 100);

        CurrentMap = OverworldMap.FromFile("Maps/Map01.json");
        MapManager.Instance.LoadDefaultMap();

        MapObjects = new();
        Drops = new();
        DropHoverTimers = new();
        DropInitialPositions = new();

        //MapObjects.Add(new MapObjectInstance<TreasureChest>(new Vector2(32 * 8, 32 * 9)));
    }

    public void Render()
    {
        var instances = new List<IInstance>();
        instances.AddRange(MapManager.Instance.Enemies);
        instances.AddRange(MapObjects);
        instances.AddRange(Drops);
        instances.Add(Player);

        instances.Sort((x, y) => x.GetDepth().CompareTo(y.GetDepth()));

        DrawMap();

        foreach (var instance in instances)
        {
            var pos = instance.GetPosition();
            var size = instance.GetSize();
            var colBox = instance.GetCollisionBox();

            if (instance is PlayerInstance playerInstance)
            {
                if (!IsAttacking)
                {
                    Rectangle sourceRectangle;
                    var flip = false;
                    var spriteSize = GameManager.PlayerSpriteSize;
                    var animFrame = IdleOrWalkingAnimFrames / 8 % 6;
                    switch (playerInstance.FacingDirection)
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
                    if (IsWalking)
                    {
                        sourceRectangle.Y += spriteSize.Y * 3;
                    }

                    GameManager.SpriteBatch.Draw(GameManager.PlayerSpriteSheet,
                        new Rectangle(pos.ToPoint(), size.ToPoint()),
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
                    var animFrame = AttackAnimFrames / AttackFrameLength;
                    switch (playerInstance.FacingDirection)
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
                    GameManager.SpriteBatch.Draw(GameManager.PlayerSpriteSheet,
                        new Rectangle(pos.ToPoint(), size.ToPoint()),
                        sourceRectangle,
                        Color.White,
                        0F,
                        Vector2.Zero,
                        flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                        0);
                }
            }
            else if (instance is IEnemyInstance enemyInstance)
            {
                if (enemyInstance.GetEnemy().GetId() == EnemyId.GreenSlime)
                {
                    GameManager.SpriteBatch.Draw(GameManager.SlimeSpriteSheet,
                        new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y),
                        new Rectangle(0, 0, 32, 32),
                        Color.White);
                }
                else
                {
                    GameManager.SpriteBatch.Draw(GameManager.SpriteSheet,
                        new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y),
                        new Rectangle(0, instance.GetSpriteSheetIndex() * 32, (int)size.X, (int)size.Y),
                        Color.White);
                }
            }
            else
            {
                GameManager.SpriteBatch.Draw(GameManager.SpriteSheet,
                    new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y),
                    new Rectangle(0, instance.GetSpriteSheetIndex() * 32, (int)size.X, (int)size.Y),
                    Color.White);
            }
        }

        var rotation = 0.0;
        var pointerPosition = Point.Zero;
        switch (Player.FacingDirection)
        {
            case Direction.Up:
                rotation = 0.0;
                pointerPosition = new Point((int)Player.Position.X + (int)Player.GetSize().X / 2,
                    (int)Player.Position.Y);
                break;
            case Direction.Down:
                rotation = 180.0 * Math.PI / 180.0;
                pointerPosition = new Point((int)Player.Position.X + (int)Player.GetSize().X / 2,
                    (int)Player.Position.Y + (int)Player.GetSize().Y);
                break;
            case Direction.Left:
                rotation = 270.0 * Math.PI / 180.0;
                pointerPosition = new Point((int)Player.Position.X,
                    (int)Player.Position.Y + (int)Player.GetSize().Y);
                break;
            case Direction.Right:
                rotation = 90.0 * Math.PI / 180.0;
                pointerPosition = new Point((int)Player.Position.X + (int)Player.GetSize().X,
                    (int)Player.Position.Y + (int)Player.GetSize().Y);
                break;
        }

        var pointerSize = new Point(32, 32);
        GameManager.SpriteBatch.Draw(GameManager.SpriteSheet,
            new Rectangle(new Point(pointerPosition.X, pointerPosition.Y), pointerSize),
            new Rectangle(new Point(0, pointerSize.Y * SpriteIndex.Pointer), pointerSize),
            Color.White,
            rotation: (float)rotation,
            origin: new Vector2(16, 16),
            effects: SpriteEffects.None,
            layerDepth: 0);

        if (DropsBelowPlayer.Count > 0)
        {
            GameManager.SpriteBatch.Draw(GameManager.Pixel,
                new Rectangle(0, 0, GameManager.Resolution.X, 50),
                new Color(0, 0, 0, 150));

            var dropName = DropsBelowPlayer.First().GetDroppable().GetName();
            var dropNameSize = GameManager.Fnt15.MeasureString(dropName);
            GameManager.SpriteBatch.DrawString(GameManager.Fnt15,
                dropName,
                new Vector2((int)(GameManager.Resolution.X / 2 - dropNameSize.X / 2),
                    (int)(50 / 2 - dropNameSize.Y / 2)),
                Color.White);
        }
    }

    private void DrawMap()
    {
        MapManager.Instance.DrawMap(GameManager.SpriteBatch);
    }

    public void CalculateCameraPosition()
    {
        GameManager.Camera.Zoom = 2F;
        var x = GameManager.Camera.BoundingRectangle;
        var mapWidth = MapManager.Instance.CurrentMap.Width * MapManager.Instance.CurrentMap.TileWidth;
        var mapHeight = MapManager.Instance.CurrentMap.Height * MapManager.Instance.CurrentMap.TileHeight;

        var cameraPos = Vector2.Zero;
        // Center the player
        cameraPos.X = Player.Position.X + Player.GetSize().X / 2;
        cameraPos.Y = Player.Position.Y + Player.GetSize().Y / 2;

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

        GameManager.Camera.LookAt(cameraPos);
    }

    public void Update(GameTime gameTime)
    {
        UpdateDrops();

        DetectAndHandleInput();
        DetectAndHandleMovement();
        DetectAndHandleCollisions();
        DetectAndHandleDropPickupEvent();
        DetectAndHandleAttack();

        CalculateCameraPosition();

        IsAboveDrop = IsPlayerAboveDropInstance(out DropsBelowPlayer);

        if (GameManager.FramesKeysHeld[Keys.C] == 1)
        {
            StateManager.Instance.PushState<CraftingMenuState>(true);
        }
        else if (GameManager.FramesKeysHeld[Keys.I] == 1)
        {
            StateManager.Instance.PushState<InventoryState>(true);
        }
        else if (GameManager.FramesKeysHeld[Keys.Q] == 1)
        {
            StateManager.Instance.PushState<QuestMenuState>(true);
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

    private void DetectAndHandleInput()
    {
        ActionKeyPressed = GameManager.FramesKeysHeld[Keys.Z] == 1;
    }

    private void DetectAndHandleMovement()
    {
        if (IsAttacking)
        {
            return;
        }

        MovementVector = Vector2.Zero;
        if (GameManager.FramesKeysHeld[Keys.Right] > 0)
        {
            MovementVector.X++;
        }
        else if (GameManager.FramesKeysHeld[Keys.Left] > 0)
        {
            MovementVector.X--;
        }

        if (GameManager.FramesKeysHeld[Keys.Up] > 0)
        {
            MovementVector.Y--;
        }
        else if (GameManager.FramesKeysHeld[Keys.Down] > 0)
        {
            MovementVector.Y++;
        }

        if (MovementVector != Vector2.Zero)
        {
            MovementVector = CustomMath.UnitVector(MovementVector);
            if (!IsWalking)
            {
                IsWalking = true;
                IdleOrWalkingAnimFrames = 0;
            }
            else
            {
                IdleOrWalkingAnimFrames++;
            }

            // If player is attacking, lock their facing direction
            if (!IsAttacking)
            {
                if (MovementVector.Y > 0)
                {
                    Player.FacingDirection = Direction.Down;
                }
                else if (MovementVector.Y < 0)
                {
                    Player.FacingDirection = Direction.Up;
                }
                else if (MovementVector.X > 0)
                {
                    Player.FacingDirection = Direction.Right;
                }
                else if (MovementVector.X < 0)
                {
                    Player.FacingDirection = Direction.Left;
                }
            }

            Player.Position.X += MovementVector.X * PlayerInstance.MovementSpeed;
            Player.Position.Y += MovementVector.Y * PlayerInstance.MovementSpeed;
        }
        else
        {
            if (IsWalking)
            {
                IsWalking = false;
                IdleOrWalkingAnimFrames = 0;
            }
            else
            {
                IdleOrWalkingAnimFrames++;
            }
        }
    }

    private void DetectAndHandleCollisions()
    {
        // TODO: eventually we would want to check for collisions on all objects that have moved on a given frame.
        var otherInstances = new List<IInstance>();
        otherInstances.AddRange(MapManager.Instance.Enemies);
        otherInstances.AddRange(MapObjects);

        // If player's movement vector is zero, default to pushing them right
        if (MovementVector == Vector2.Zero)
        {
            MovementVector = new Vector2(1, 0);
        }

        // Check collision with other instance, e.g. enemies and objects
        foreach (var instance in otherInstances)
        {
            var otherColBox = instance.GetCollisionBox();
            while (otherColBox.Intersects(Player.GetCollisionBox()))
            {
                Player.Position.X -= MovementVector.X;
                Player.Position.Y -= MovementVector.Y;
            }
        }

        // Check collision with solid map tiles
        // var tiles = GetTilesBelowPlayer();
        // foreach (var tile in tiles)
        // {
        //     if (CurrentMap.CollisionMap[tile.Y][tile.X] == 2)
        //     {
        //         var tileCollisionBox = new Rectangle(tile.X * 32, tile.Y * 32, 32, 32);
        //         while (tileCollisionBox.Intersects(Player.GetCollisionBox()))
        //         {
        //             Player.Position.X -= MovementVector.X;
        //             Player.Position.Y -= MovementVector.Y;
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
        if (ActionKeyPressed && IsAboveDrop)
        {
            var drop = DropsBelowPlayer.First();
            drop.GetDroppable().OnObtain();
            var i = Drops.IndexOf(drop);
            DropsBelowPlayer.Remove(drop);
            Drops.Remove(drop);
            DropInitialPositions.RemoveAt(i);
            DropHoverTimers.RemoveAt(i);
            ActionKeyPressed = false;
        }
    }

    private void DetectAndHandleAttack()
    {
        if (IsAttacking)
        {
            AttackAnimFrames++;
            if (AttackAnimFrames / AttackFrameLength > 3)
            {
                IsAttacking = false;
                AttackedEnemies.Clear();
                return;
            }

            // Check if attack collides with any enemies
            CalculateAttackHitbox();

            foreach (var inst in MapManager.Instance.Enemies)
            {
                if (!AttackedEnemies.Contains(inst) && inst.GetCollisionBox().Intersects(AttackRect))
                {
                    GameManager.HitSfx01.Play(0.3F, 0F, 0F);
                    var damage = Player.Info.Equipment.Weapon.GetAttackStat();
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

            MapManager.Instance.Enemies.RemoveAll(x => x.GetCurrentHitPoints() <= 0);
        }
        else
        {
            if (ActionKeyPressed)
            {
                IsAttacking = true;
                AttackAnimFrames = 0;
                CalculateAttackHitbox();
                ActionKeyPressed = false;
                GameManager.SwingSfx01.Play(volume: 0.1F, 0F, 0F);
            }
        }
    }

    private void CalculateAttackHitbox()
    {
        var pSize = Player.GetSize().ToPoint();
        var pPos = Player.Position.ToPoint();
        var animFrame = AttackAnimFrames / AttackFrameLength;
        if (Player.FacingDirection == Direction.Left)
        {
            AttackRect = animFrame switch
            {
                0 => new Rectangle(new Point(0, 0), new Point(0, 0)),
                1 => new Rectangle(new Point(pPos.X + 6, pPos.Y + 33), new Point(13, 14)),
                2 => new Rectangle(0, 0, 0, 0),
                3 => new Rectangle(0, 0, 0, 0),
                _ => AttackRect
            };
        }
        else if (Player.FacingDirection == Direction.Right)
        {
            AttackRect = animFrame switch
            {
                0 => new Rectangle(new Point(0, 0), new Point(0, 0)),
                1 => new Rectangle(new Point(pPos.X + 32, pPos.Y + 32), new Point(13, 14)),
                2 => new Rectangle(0, 0, 0, 0),
                3 => new Rectangle(0, 0, 0, 0),
                _ => AttackRect
            };
        }
        else if (Player.FacingDirection == Direction.Down)
        {
            AttackRect = animFrame switch
            {
                0 => new Rectangle(new Point(0, 0), new Point(0, 0)),
                1 => new Rectangle(pPos.X + 17, pPos.Y + 37, 22, 11),
                2 => new Rectangle(0, 0, 0, 0),
                3 => new Rectangle(0, 0, 0, 0),
                _ => AttackRect
            };
        }
        else if (Player.FacingDirection == Direction.Up)
        {
            AttackRect = animFrame switch
            {
                0 => new Rectangle(new Point(0, 0), new Point(0, 0)),
                1 => new Rectangle(pPos.X + 11, pPos.Y + 22, 22, 11),
                2 => new Rectangle(0, 0, 0, 0),
                3 => new Rectangle(0, 0, 0, 0),
                _ => AttackRect
            };
        }
    }

    private bool IsPlayerAboveDropInstance(out List<IDropInstance> drops)
    {
        drops = new List<IDropInstance>();

        var playerBounds = Player.GetBounds();

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

    private HashSet<Point> GetTilesBelowPlayer()
    {
        var tiles = new HashSet<Point>();
        var pcb = Player.GetCollisionBox();

        tiles.Add(new(pcb.X / 32, pcb.Y / 32));
        tiles.Add(new((pcb.X + 32) / 32, pcb.Y / 32));
        tiles.Add(new(pcb.X / 32, (pcb.Y + 32) / 32));
        tiles.Add(new((pcb.X + 32) / 32, (pcb.Y + 32) / 32));

        return tiles;
    }
}