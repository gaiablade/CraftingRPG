using CraftingRPG.Constants;
using CraftingRPG.Enemies;
using CraftingRPG.Entities;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using CraftingRPG.MapObjects;
using CraftingRPG.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CraftingRPG.States;

public class OverworldState : IState
{
    private PlayerInstance Player;
    private List<IEnemyInstance> Enemies;
    private List<IInstance> MapObjects;
    private Vector2 MovementVector;

    public OverworldState()
    {
        GameManager.AddKeysIfNotExists(Keys.Left, Keys.Right, Keys.Up, Keys.Down, Keys.Z, Keys.X, Keys.C, Keys.I);

        Player = new PlayerInstance(GameManager.PlayerInfo);
        Player.Position = new Vector2(100, 100);

        Enemies = new List<IEnemyInstance>
        {
            new EnemyInstance<GreenSlime>(new(), new Vector2(500, 500)),
            new EnemyInstance<GreenSlime>(new(), new Vector2(200, 300)),
            new EnemyInstance<GreenSlime>(new(), new Vector2(700, 100))
        };

        MapObjects = new();
        for (int i = 0; i < 10; i++)
        {
            MapObjects.Add(new MapObjectInstance<Crate>(new Vector2(
                Random.Shared.Next() % GameManager.Resolution.X,
                Random.Shared.Next() % GameManager.Resolution.Y)));
        }
    }

    public void Render()
    {
        var instances = new List<IInstance>();
        instances.AddRange(Enemies);
        instances.AddRange(MapObjects);
        instances.Add(Player);

        instances.Sort((x, y) => x.GetDepth().CompareTo(y.GetDepth()));

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
    }

    public void Update()
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

            Player.Position.X += MovementVector.X * PlayerInstance.MovementSpeed;
            Player.Position.Y += MovementVector.Y * PlayerInstance.MovementSpeed;
        }

        // check for collisions
        var otherInstances = new List<IInstance>();
        otherInstances.AddRange(Enemies);
        otherInstances.AddRange(MapObjects);

        foreach (var instance in otherInstances)
        {
            var otherColBox = instance.GetCollisionBox();
            while (otherColBox.Intersects(Player.GetCollisionBox()))
            {
                Player.Position.X -= MovementVector.X;
                Player.Position.Y -= MovementVector.Y;
            }
        }


        if (GameManager.FramesKeysHeld[Keys.C] == 1)
        {
            StateManager.Instance.PushState<CraftingMenuState>(true);
        }
        else if (GameManager.FramesKeysHeld[Keys.I] == 1)
        {
            StateManager.Instance.PushState<InventoryState>(true);
        }
    }
}