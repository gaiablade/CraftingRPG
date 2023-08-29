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
using System.Linq;

namespace CraftingRPG.States;

public class OverworldState : IState
{
    private ToState toState;
    public ToState GetToState() => toState;

    //private PlayerInfo PlayerInfo;
    private PlayerInstance Player;
    private List<IEnemyInstance> Enemies;
    private List<IInstance> MapObjects;

    public OverworldState()
    {
        GameManager.AddKeysIfNotExists(Keys.Left, Keys.Right, Keys.Up, Keys.Down, Keys.Z, Keys.X);

        Player = new PlayerInstance(GameManager.PlayerInfo);
        Player.Position = new Point(100, 100);

        Enemies = new List<IEnemyInstance>
        {
            new EnemyInstance<GreenSlime>(new(), new Point(500, 500)),
            new EnemyInstance<GreenSlime>(new(), new Point(200, 300)),
            new EnemyInstance<GreenSlime>(new(), new Point(700, 100))
        };

        MapObjects = new List<IInstance>
        {
            new MapObjectInstance<Crate>(new Point(32, 200)),
            new MapObjectInstance<Crate>(new Point(64, 200)),
            new MapObjectInstance<Crate>(new Point(96, 200)),
            new MapObjectInstance<Crate>(new Point(128, 200)),
            new MapObjectInstance<Crate>(new Point(0, 200)),
            new MapObjectInstance<Crate>(new Point(160, 200)),
        };
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
            GameManager.SpriteBatch.Draw(GameManager.SpriteSheet,
                new Rectangle(pos.X, pos.Y, (int)size.X, (int)size.Y),
                new Rectangle(0, instance.GetSpriteSheetIndex() * 32, (int)size.X, (int)size.Y),
                Color.White);
        }

        var rotation = 0.0;
        var pointerPosition = Point.Zero;
        switch (Player.FacingDirection)
        {
            case Direction.Up:
                rotation = 0.0;
                pointerPosition = new Point(Player.Position.X, Player.Position.Y - 32);
                break;
            case Direction.Down:
                rotation = 180.0 * Math.PI / 180.0;
                pointerPosition = new Point(Player.Position.X, Player.Position.Y + 64);
                break;
            case Direction.Left:
                rotation = 270.0 * Math.PI / 180.0;
                pointerPosition = new Point(Player.Position.X - 32, Player.Position.Y + 32);
                break;
            case Direction.Right:
                rotation = 90.0 * Math.PI / 180.0;
                pointerPosition = new Point(Player.Position.X + 32, Player.Position.Y + 32);
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
        var movVector = Vector2.Zero;
        if (GameManager.FramesKeysHeld[Keys.Right] > 0)
        {
            movVector.X++;
        }
        else if (GameManager.FramesKeysHeld[Keys.Left] > 0)
        {
            movVector.X--;
        }
        if (GameManager.FramesKeysHeld[Keys.Up] > 0)
        {
            movVector.Y--;
        }
        else if (GameManager.FramesKeysHeld[Keys.Down] > 0)
        {
            movVector.Y++;
        }

        if (movVector != Vector2.Zero)
        {
            movVector = CustomMath.UnitVector(movVector);

            if (movVector.Y > 0)
            {
                Player.FacingDirection = Direction.Down;
            }
            else if (movVector.Y < 0)
            {
                Player.FacingDirection = Direction.Up;
            }
            else if (movVector.X > 0)
            {
                Player.FacingDirection = Direction.Right;
            }
            else if (movVector.X < 0)
            {
                Player.FacingDirection = Direction.Left;
            }

            movVector.X *= PlayerInstance.MovementSpeed;
            movVector.Y *= PlayerInstance.MovementSpeed;

            Player.Position.X += (int)movVector.X;
            Player.Position.Y += (int)movVector.Y;
        }

        if (GameManager.FramesKeysHeld[Keys.Enter] == 1)
        {
            toState = ToState.CraftingMenu;
        }
    }
}
