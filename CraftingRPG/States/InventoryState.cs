using CraftingRPG.Constants;
using CraftingRPG.Entities;
using CraftingRPG.Interfaces;
using CraftingRPG.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CraftingRPG.States;

public class InventoryState : IState
{
    private const int NumberOfColumns = 6;
    private readonly List<string> Tabs = new()
    {
        "All (Unsorted)",
        "Weapons",
        "Armor",
        "Potions",
        "Ingredients"
    };

    private Inventory Inventory;
    private PlayerEquipment Equipment;
    private int ActiveTab = 0;
    private Point Cursor = new(0, 0);

    // Drawing constants
    private readonly int CenterX = GameManager.Resolution.X / 2;
    private readonly int ItemWidthAndGap = 32 + 30;
    private readonly int ItemHeightAndGap = 32 + 10;
    private readonly int GridTop = 150;

    public InventoryState()
    {
        GameManager.AddKeysIfNotExists(Keys.LeftControl, Keys.Left, Keys.Right, Keys.Down, Keys.Up);

        Inventory = GameManager.PlayerInfo.Inventory;
        Equipment = GameManager.PlayerInfo.Equipment;
    }

    public void Render()
    {
        DrawEquipment();
        DrawItems();

        // DEBUG: Draw a separator in the center of the screen
        GameManager.SpriteBatch.Draw(GameManager.Pixel,
            new Rectangle(GameManager.Resolution.X / 2, 0, 1, GameManager.Resolution.Y),
            Color.Black);
    }

    private void DrawEquipment()
    {
        var fntH = GameManager.Fnt12.MeasureString("A").Y;

        var header = "Equipment";
        var headerSize = GameManager.Fnt20.MeasureString(header);
        GameManager.SpriteBatch.DrawString(GameManager.Fnt20,
            header,
            new Vector2(CenterX / 2 - headerSize.X / 2, 25),
            Color.White);

        GameManager.SpriteBatch.DrawString(GameManager.Fnt12,
            "Weapon",
            new Vector2(50, 75),
            Color.White);

        if (Equipment.Weapon != null)
        {
            GameManager.SpriteBatch.Draw(GameManager.SpriteSheet,
                new Rectangle(50, (int)(75 + fntH + 10), 32, 32),
                new Rectangle(0, Equipment.Weapon.GetSpriteSheetIndex() * 32, 32, 32),
                Color.White);
            GameManager.SpriteBatch.DrawString(GameManager.Fnt12,
                Equipment.Weapon.GetName(),
                new Vector2(50 + 32 + 10, 75 + fntH + 10 + 16 - fntH / 2),
                Color.White);
        }
        else
        {
            GameManager.SpriteBatch.DrawString(GameManager.Fnt12,
                "None",
                new Vector2(50, 75 + fntH + 10),
                Color.DarkGray);
        }
    }

    private void DrawItems()
    {
        var header = "Items";
        var headerSize = GameManager.Fnt20.MeasureString(header);
        GameManager.SpriteBatch.DrawString(GameManager.Fnt20,
            header,
            new Vector2(GameManager.Resolution.X / 2 + GameManager.Resolution.X / 4 - headerSize.X / 2, 25),
            Color.White);

        var gridMaxW = NumberOfColumns * ItemWidthAndGap;
        var gridX = GameManager.Resolution.X / 2 + (GameManager.Resolution.X / 4 - gridMaxW / 2);

        GameManager.SpriteBatch.Draw(GameManager.Pixel,
            new Rectangle(gridX, GridTop - 16, gridMaxW, 32 + 8 * 32), 
            Color.Red);

        if (Inventory.Items.Count > 0)
        {
            var row = 0;
            var col = 0;
            foreach (var (itemId, qty) in Inventory.Items)
            {
                var itemInfo = GameManager.ItemInfo[itemId];
                GameManager.SpriteBatch.Draw(GameManager.SpriteSheet,
                    new Rectangle(gridX + 16 + col * ItemWidthAndGap, GridTop + row * ItemHeightAndGap, 32, 32),
                    new Rectangle(0, itemInfo.GetSpriteSheetIndex() * 32, 32, 32),
                    Color.White);
                GameManager.SpriteBatch.DrawString(GameManager.Fnt12,
                    "x" + qty.ToString(),
                    new Vector2(gridX + 16 + col * ItemWidthAndGap, GridTop + row * ItemHeightAndGap),
                    Color.White);

                col++;
                if (col == NumberOfColumns)
                {
                    col = 0;
                    row++;
                }
            }

            DrawCursor(gridX);
            DrawSelectedItemName();
        }
    }

    public void DrawCursor(int gridX)
    {
        var cursorX = gridX + 16 + Cursor.X * ItemWidthAndGap;
        var cursorY = GridTop + Cursor.Y * ItemHeightAndGap;
        GameManager.SpriteBatch.Draw(GameManager.SpriteSheet,
            new Rectangle(cursorX, cursorY, 32, 32),
            new Rectangle(0, SpriteIndex.Cursor * 32, 32, 32),
            Color.White);
    }

    public void DrawSelectedItemName()
    {
        var cursorFlat = Cursor.Y * NumberOfColumns + Cursor.X;
        var (itemId, qty) = Inventory.Items.ElementAt(cursorFlat);
        var itemInfo = GameManager.ItemInfo[itemId];
        var itemName = itemInfo.GetName() + " x" + qty;
        var itemNameSize = GameManager.Fnt12.MeasureString(itemName);
        GameManager.SpriteBatch.DrawString(GameManager.Fnt12,
            itemName,
            new Vector2(CenterX + CenterX / 2 - itemNameSize.X / 2, 100),
            Color.White);
    }

    public void Update()
    {
        if (GameManager.FramesKeysHeld[Keys.LeftControl] == 1)
        {
            StateManager.Instance.PopState();
        }

        if (GameManager.FramesKeysHeld[Keys.Left] == 1)
        {
            Cursor.X = CustomMath.WrapAround(Cursor.X - 1, 0, NumberOfColumns - 1);
        }
        else if (GameManager.FramesKeysHeld[Keys.Right] == 1)
        {
            Cursor.X = CustomMath.WrapAround(Cursor.X + 1, 0, NumberOfColumns - 1);
        }
        else if (GameManager.FramesKeysHeld[Keys.Down] == 1)
        {
            Cursor.Y++;
        }
        else if (GameManager.FramesKeysHeld[Keys.Up] == 1)
        {
            Cursor.Y--;
        }

        NormalizeCursor();
    }

    private void NormalizeCursor()
    {
        var numRows = Inventory.Items.Count / NumberOfColumns + 1;
        while (Cursor.Y >= numRows)
        {
            Cursor.Y--;
        }
        if (Cursor.Y < 0)
        {
            Cursor.Y = 0;
        }
        // If on last row and Cursor.X greater than number of columns in row, set to last
        if (Cursor.Y == numRows - 1)
        {
            var lastRowColumns = Inventory.Items.Count % NumberOfColumns;
            if (Cursor.X > lastRowColumns - 1)
            {
                Cursor.X = lastRowColumns - 1;
            }
        }
    }
}
