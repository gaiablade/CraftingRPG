using CraftingRPG.Constants;
using CraftingRPG.Entities;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using CraftingRPG.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace CraftingRPG.States;

public class MapEditorState : IState
{
    private Point Position = Point.Zero;
    private Point Dimensions;
    private int CurrentTile = 0;
    private OverworldMap Map;

    public MapEditorState()
    {
        var columns = GameManager.Resolution.X / 32;
        var rows = GameManager.Resolution.Y / 32;
        Dimensions = new Point(columns, rows);
        Map = new OverworldMap(columns, rows);

        GameManager.AddKeysIfNotExists(Keys.Left, Keys.Right, Keys.Up, Keys.Down, Keys.Z, Keys.Space, Keys.X, Keys.Q, Keys.W);
    }

    public void Render()
    {
        // Draw Map
        for (int y = 0; y < Map.Tiles.Count; y++)
        {
            for (int x = 0; x < Map.Tiles[y].Count; x++)
            {
                GameManager.SpriteBatch.Draw(GameManager.TileSet, 
                    new Rectangle(32 * x, 32 * y, 32, 32), 
                    new Rectangle(0, 32 * Map.Tiles[y][x], 32, 32), 
                    Color.White);
            }
        }

        if (32 * CurrentTile >= 0 && 32 * CurrentTile < GameManager.TileSet.Height)
        {
            GameManager.SpriteBatch.Draw(GameManager.TileSet,
                new Rectangle(32 * Position.X, 32 * Position.Y, 32, 32),
                new Rectangle(0, 32 * CurrentTile, 32, 32),
                Color.White);
        }
        GameManager.SpriteBatch.Draw(GameManager.SpriteSheet,
            new Rectangle(32 * Position.X, 32 * Position.Y, 32, 32),
            new Rectangle(0, 32 * SpriteIndex.Cursor, 32, 32),
            Color.White);
    }

    public void Update()
    {
        if (GameManager.FramesKeysHeld[Keys.Left] == 1)
        {
            Position.X = CustomMath.WrapAround(Position.X - 1, 0, Dimensions.X - 1);
        }
        else if (GameManager.FramesKeysHeld[Keys.Right] == 1)
        {
            Position.X = CustomMath.WrapAround(Position.X + 1, 0, Dimensions.X - 1);
        }
        else if (GameManager.FramesKeysHeld[Keys.Down] == 1)
        {
            Position.Y = CustomMath.WrapAround(Position.Y + 1, 0, Dimensions.Y - 1);
        }
        else if (GameManager.FramesKeysHeld[Keys.Up] == 1)
        {
            Position.Y = CustomMath.WrapAround(Position.Y - 1, 0, Dimensions.Y - 1);
        }

        if (GameManager.FramesKeysHeld[Keys.Q] == 1)
        {
            CurrentTile--;
        }
        else if (GameManager.FramesKeysHeld[Keys.W] == 1)
        {
            CurrentTile++;
        }

        if (GameManager.FramesKeysHeld[Keys.Z] == 1)
        {
            Map.Tiles[Position.Y][Position.X] = CurrentTile;
        }
    }    
}
