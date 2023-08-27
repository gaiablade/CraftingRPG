using CraftingRPG.Enemies;
using CraftingRPG.Entities;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using CraftingRPG.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace CraftingRPG.States;

public class OverworldState : IState
{
    private ToState toState;
    public ToState GetToState() => toState;

    private Player Player;
    private List<IEnemyInstance> Enemies;

    public OverworldState()
    {
        GameManager.AddKeysIfNotExists(Keys.Left, Keys.Right, Keys.Up, Keys.Down, Keys.Z, Keys.X);

        Player = GameManager.Player;
        Player.Position = new Point(100, 100);

        Enemies = new List<IEnemyInstance>
        {
            new EnemyInstance<GreenSlime>(new(), new Point(500, 500)),
            new EnemyInstance<GreenSlime>(new(), new Point(200, 300)),
            new EnemyInstance<GreenSlime>(new(), new Point(700, 100))
        };
    }

    public void Render()
    {
        // draw player
        GameManager.SpriteBatch.Draw(GameManager.SpriteSheet,
            new Rectangle(Player.Position.X, Player.Position.Y, 32, 32),
            new Rectangle(0, 0, 32, 32),
            Color.White);

        // draw enemies
        foreach (var enemy in Enemies)
        {
            var instance = enemy.GetEnemy();
            GameManager.SpriteBatch.Draw(GameManager.SpriteSheet,
                new Rectangle(enemy.GetPosition().X, enemy.GetPosition().Y, 32, 32),
                new Rectangle(0, 32 * instance.GetSpriteSheetIndex(), 32, 32),
                Color.White);
        }
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
            movVector.X *= Player.MovementSpeed;
            movVector.Y *= Player.MovementSpeed;

            Player.Position.X += (int)movVector.X;
            Player.Position.Y += (int)movVector.Y;
        }

        if (GameManager.FramesKeysHeld[Keys.Enter] == 1)
        {
            toState = ToState.CraftingMenu;
        }
    }
}
