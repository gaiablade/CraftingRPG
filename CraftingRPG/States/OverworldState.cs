using CraftingRPG.Constants;
using CraftingRPG.Enemies;
using CraftingRPG.Entities;
using CraftingRPG.Interfaces;
using CraftingRPG.MapObjects;
using CraftingRPG.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

// I love you

namespace CraftingRPG.States;

public class OverworldState : IState
{
    private PlayerInstance Player;
    private List<IEnemyInstance> Enemies;
    private List<IInstance> MapObjects;
    private List<IDropInstance> Drops;
    private Vector2 MovementVector;
    private bool IsAttacking = false;
    private int AttackFrame = 0;
    private Rectangle AttackRect;
    private List<IEnemyInstance> AttackedEnemies = new();
    private bool IsAboveDrop = false;
    private List<IDropInstance> DropsBelowPlayer;
    private bool ActionKeyPressed = false;
    private OverworldMap CurrentMap;

    public OverworldState()
    {
        GameManager.AddKeysIfNotExists(Keys.Left, Keys.Right, Keys.Up, Keys.Down, Keys.Z, Keys.X, Keys.C, Keys.I);

        Player = new PlayerInstance(GameManager.PlayerInfo);
        Player.Position = new Vector2(100, 100);

        CurrentMap = OverworldMap.FromFile("Maps/Map01.json");

        Enemies = new List<IEnemyInstance>
        {
            new EnemyInstance<GreenSlime>(new(), new Vector2(500, 500)),
            new EnemyInstance<GreenSlime>(new(), new Vector2(200, 300)),
            new EnemyInstance<GreenSlime>(new(), new Vector2(700, 100))
        };

        MapObjects = new();
        Drops = new();
    }

    public void Render()
    {
        var instances = new List<IInstance>();
        instances.AddRange(Enemies);
        instances.AddRange(MapObjects);
        instances.AddRange(Drops);
        instances.Add(Player);

        instances.Sort((x, y) => x.GetDepth().CompareTo(y.GetDepth()));

        DrawMap();

        if (IsAttacking)
        {
            GameManager.SpriteBatch.Draw(GameManager.Pixel,
                AttackRect,
                Color.Red);
        }

        foreach (var instance in instances)
        {
            var pos = instance.GetPosition();
            var size = instance.GetSize();
            var colBox = instance.GetCollisionBox();
            GameManager.SpriteBatch.Draw(GameManager.SpriteSheet,
                new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y),
                new Rectangle(0, instance.GetSpriteSheetIndex() * 32, (int)size.X, (int)size.Y),
                Color.White);
        }

        var rotation = 0.0;
        var pointerPosition = Point.Zero;
        switch (Player.FacingDirection)
        {
            case Direction.Up:
                rotation = 0.0;
                pointerPosition = new Point((int)Player.Position.X, (int)Player.Position.Y - 32);
                break;
            case Direction.Down:
                rotation = 180.0 * Math.PI / 180.0;
                pointerPosition = new Point((int)Player.Position.X, (int)Player.Position.Y + 64);
                break;
            case Direction.Left:
                rotation = 270.0 * Math.PI / 180.0;
                pointerPosition = new Point((int)Player.Position.X - 32, (int)Player.Position.Y + 32);
                break;
            case Direction.Right:
                rotation = 90.0 * Math.PI / 180.0;
                pointerPosition = new Point((int)Player.Position.X + 32, (int)Player.Position.Y + 32);
                break;
        }
        GameManager.SpriteBatch.Draw(GameManager.SpriteSheet,
            new Rectangle(pointerPosition.X + 16, pointerPosition.Y + 16, 32, 32),
            new Rectangle(0, 32 * SpriteIndex.Pointer, 32, 32),
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
                new Vector2(GameManager.Resolution.X / 2 - dropNameSize.X / 2, 50 / 2 - dropNameSize.Y / 2),
                Color.White);
        }
    }

    private void DrawMap()
    {
        for (int y = 0; y < CurrentMap.Height; y++)
        {
            for (int x = 0; x < CurrentMap.Width; x++)
            {
                if (CurrentMap.Tiles[y][x] != -1)
                {
                    GameManager.SpriteBatch.Draw(GameManager.TileSet,
                        new Rectangle(32 * x, 32 * y, 32, 32),
                        new Rectangle(0, 32 * CurrentMap.Tiles[y][x], 32, 32),
                        Color.White);
                }
            }
        }
    }

    public void Update()
    {
        DetectAndHandleInput();
        DetectAndHandleMovement();
        DetectAndHandleCollisions();
        DetectAndHandleDropPickupEvent();
        DetectAndHandleAttack();

        IsAboveDrop = IsPlayerAboveDropInstance(out DropsBelowPlayer);

        if (GameManager.FramesKeysHeld[Keys.C] == 1)
        {
            StateManager.Instance.PushState<CraftingMenuState>(true);
        }
        else if (GameManager.FramesKeysHeld[Keys.I] == 1)
        {
            StateManager.Instance.PushState<InventoryState>(true);
        }
    }

    private void DetectAndHandleInput()
    {
        ActionKeyPressed = GameManager.FramesKeysHeld[Keys.Z] == 1;
    }

    private void DetectAndHandleMovement()
    {
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
    }

    private void DetectAndHandleCollisions()
    {
        // TODO: eventually we would want to check for collisions on all objects that have moved on a given frame.
        var otherInstances = new List<IInstance>();
        otherInstances.AddRange(Enemies);
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
        var tiles = GetTilesBelowPlayer();
        foreach (var tile in tiles)
        {
            if (CurrentMap.CollisionMap[tile.Y][tile.X] == 2)
            {
                var tileCollisionBox = new Rectangle(tile.X * 32, tile.Y * 32, 32, 32);
                while (tileCollisionBox.Intersects(Player.GetCollisionBox()))
                {
                    Player.Position.X -= MovementVector.X;
                    Player.Position.Y -= MovementVector.Y;
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
            DropsBelowPlayer.Remove(drop);
            Drops.Remove(drop);
            ActionKeyPressed = false;
        }
    }

    private void DetectAndHandleAttack()
    {
        if (IsAttacking)
        {
            // Check if attack collides with any enemies
            CalculateAttackHitbox();

            foreach (var inst in Enemies)
            {
                if (!AttackedEnemies.Contains(inst) && inst.GetCollisionBox().Intersects(AttackRect))
                {
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
                                var dropInstance = new DropInstance(possibleDrop.Drop, inst.GetPosition());
                                Drops.Add(dropInstance);
                            }
                        }
                    }
                }
            }
            Enemies.RemoveAll(x => x.GetCurrentHitPoints() <= 0);

            AttackFrame++;
            if (AttackFrame > 30)
            {
                IsAttacking = false;
                AttackFrame = 0;
                AttackedEnemies.Clear();
            }
        }
        else
        {
            if (ActionKeyPressed)
            {
                IsAttacking = true;
                CalculateAttackHitbox();
                ActionKeyPressed = false;
            }
        }
    }

    private void CalculateAttackHitbox()
    {
        AttackRect = Player.FacingDirection switch
        {
            Direction.Left => new Rectangle((int)Player.Position.X - 32, (int)Player.Position.Y + 16, 32, 32),
            Direction.Right => new Rectangle((int)Player.Position.X + 32, (int)Player.Position.Y + 16, 32, 32),
            Direction.Up => new Rectangle((int)Player.Position.X, (int)Player.Position.Y, 32, 32),
            Direction.Down => new Rectangle((int)Player.Position.X, (int)Player.Position.Y + 64, 32, 32),
            _ => new Rectangle()
        };
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
