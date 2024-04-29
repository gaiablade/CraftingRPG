using CraftingRPG.Constants;
using CraftingRPG.Entities;
using CraftingRPG.Interfaces;
using CraftingRPG.Utility;
using Microsoft.Xna.Framework;
using System.IO;
using CraftingRPG.Enums;
using CraftingRPG.InputManagement;

namespace CraftingRPG.States;

public class MapEditorState : IState
{
    private Point Position = Point.Zero;
    private int CursorWidth = 1;
    private int CursorHeight = 1;
    private Point Dimensions;
    private int CurrentTile = 0;
    private int CurrentCollision = 0;
    private OverworldMap Map;
    private int Mode = 0;

    public MapEditorState()
    {
        var columns = GameManager.Resolution.X / 32;
        var rows = GameManager.Resolution.Y / 32;
        Dimensions = new Point(columns, rows);
        Map = new OverworldMap(columns, rows);
    }

    public void DrawWorld()
    {
        // Draw Map
        for (int y = 0; y < Map.Tiles.Count; y++)
        {
            for (int x = 0; x < Map.Tiles[y].Count; x++)
            {
                if (Map.Tiles[y][x] != -1)
                {
                    GameManager.SpriteBatch.Draw(GameManager.TileSet,
                        new Rectangle(32 * x, 32 * y, 32, 32),
                        new Rectangle(0, 32 * Map.Tiles[y][x], 32, 32),
                        Color.White);
                }
            }
        }

        // If in tile mode, draw selected tile and cursor
        // If in collision mode, draw collision and cursor
        if (Mode == 0)
        {
            for (int y = 0; y < CursorHeight; y++)
            {
                for (int x = 0; x < CursorWidth; x++)
                {
                    if (32 * CurrentTile >= 0 && 32 * CurrentTile < GameManager.TileSet.Height)
                    {
                        GameManager.SpriteBatch.Draw(GameManager.TileSet,
                            new Rectangle(32 * (Position.X + x), 32 * (Position.Y + y), 32, 32),
                            new Rectangle(0, 32 * CurrentTile, 32, 32),
                            Color.White);
                    }
                    GameManager.SpriteBatch.Draw(GameManager.SpriteSheet,
                        new Rectangle(32 * (Position.X + x), 32 * (Position.Y + y), 32, 32),
                        new Rectangle(0, 32 * SpriteIndex.Cursor, 32, 32),
                        Color.White);
                }
            }
        }
        else if (Mode == 1)
        {
            for (int y = 0; y < Map.Height; y++)
            {
                for (int x = 0; x < Map.Width; x++)
                {
                    var col = Map.CollisionMap[y][x];
                    var color = col switch
                    {
                        1 => Color.Green,
                        2 => Color.Red,
                        _ => Color.Gray
                    };
                    GameManager.SpriteBatch.Draw(GameManager.Pixel,
                        new Rectangle(32 * x, 32 * y, 32, 32),
                        color * 0.5F);
                }
            }
            for (int y = 0; y < CursorHeight; y++)
            {
                for (int x = 0; x < CursorWidth; x++)
                {
                    var color = CurrentCollision switch
                    {
                        1 => Color.Green,
                        2 => Color.Red,
                        _ => Color.Gray
                    };
                    GameManager.SpriteBatch.Draw(GameManager.Pixel,
                        new Rectangle(32 * (Position.X + x), 32 * (Position.Y + y), 32, 32),
                        color * 0.5F);
                    GameManager.SpriteBatch.Draw(GameManager.SpriteSheet,
                        new Rectangle(32 * (Position.X + x), 32 * (Position.Y + y), 32, 32),
                        new Rectangle(0, 32 * SpriteIndex.Cursor, 32, 32),
                        Color.White);
                }
            }
        }
    }

    public void DrawUI()
    {
    }

    public void Update(GameTime gameTime)
    {
        if (InputManager.Instance.IsKeyPressed(InputAction.OpenInventoryMenu))
        {
            if (Mode == 0)
            {
                Mode = 1;
            }
            else if (Mode == 1)
            {
                Mode = 0;
            }
        }

        if (InputManager.Instance.IsKeyPressed(InputAction.MoveWest))
        {
            Position.X = CustomMath.WrapAround(Position.X - 1, 0, Dimensions.X - 1);
        }
        else if (InputManager.Instance.IsKeyPressed(InputAction.MoveEast))
        {
            Position.X = CustomMath.WrapAround(Position.X + 1, 0, Dimensions.X - 1);
        }
        else if (InputManager.Instance.IsKeyPressed(InputAction.MoveSouth))
        {
            Position.Y = CustomMath.WrapAround(Position.Y + 1, 0, Dimensions.Y - 1);
        }
        else if (InputManager.Instance.IsKeyPressed(InputAction.MoveNorth))
        {
            Position.Y = CustomMath.WrapAround(Position.Y - 1, 0, Dimensions.Y - 1);
        }

        if (InputManager.Instance.IsKeyPressed(InputAction.OpenQuestsMenu))
        {
            if (Mode == 0)
            {
                CurrentTile--;
            }
            else if (Mode == 1)
            {
                CurrentCollision = CustomMath.WrapAround(CurrentCollision - 1, 1, 2);
            }
        }
        else if (InputManager.Instance.IsKeyPressed(InputAction.Attack))
        {
            if (Mode == 0)
            {
                CurrentTile++;
            }
            else if (Mode == 1)
            {
                CurrentCollision = CustomMath.WrapAround(CurrentCollision + 1, 1, 2);
            }
        }

        if (InputManager.Instance.IsKeyPressed(InputAction.Attack))
        {
            CursorWidth--;
        }
        else if (InputManager.Instance.IsKeyPressed(InputAction.Attack))
        {
            CursorWidth++;
        }

        if (InputManager.Instance.IsKeyPressed(InputAction.Attack))
        {
            CursorHeight--;
        }
        else if (InputManager.Instance.IsKeyPressed(InputAction.Attack))
        {
            CursorHeight++;
        }

        if (InputManager.Instance.IsKeyPressed(InputAction.Attack))
        {
            if (Mode == 0)
            {
                for (int y = 0; y < CursorHeight; y++)
                {
                    for (int x = 0; x < CursorWidth; x++)
                    {
                        Map.Tiles[Position.Y + y][Position.X + x] = CurrentTile;
                    }
                }
            }
            else if (Mode == 1)
            {
                for (int y = 0; y < CursorHeight; y++)
                {
                    for (int x = 0; x < CursorWidth; x++)
                    {
                        Map.CollisionMap[Position.Y + y][Position.X + x] = CurrentCollision;
                    }
                }
            }
        }

        if (InputManager.Instance.IsKeyPressed(InputAction.Attack))
        {
            // save
            var json = Map.Serialize();
            if (!Directory.Exists("mapcreator"))
            {
                Directory.CreateDirectory("mapcreator");
            }
            File.WriteAllText("mapcreator/outputmap.json", json);
        }
    }
}
