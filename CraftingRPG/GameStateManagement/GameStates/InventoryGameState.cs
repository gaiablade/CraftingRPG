﻿using System.Linq;
using CraftingRPG.AssetManagement;
using CraftingRPG.Enums;
using CraftingRPG.Extensions;
using CraftingRPG.Global;
using CraftingRPG.InputManagement;
using CraftingRPG.Interfaces;
using CraftingRPG.SpriteAnimation;
using CraftingRPG.Timers;
using CraftingRPG.Utility;
using Microsoft.Xna.Framework;

namespace CraftingRPG.GameStateManagement.GameStates;

public class InventoryGameState : BaseGameState
{
    private const int NumberOfColumns = 5;

    private int Cursor;

    private Point CursorGridPosition => new Point(Cursor % NumberOfColumns, Cursor / NumberOfColumns);

    private ITimer TransitionTimer;
    private double MenuPosition;
    private bool MenuClosed;

    private Animation CursorAnimation;

    public InventoryGameState()
    {
        TransitionTimer = new EaseOutTimer(0.5);
        CursorAnimation = new Animation(4, 0.4, new Point(32, 32));
    }

    public override void DrawWorld()
    {
    }

    public override void DrawUi()
    {
        var percent = TransitionTimer.GetPercent();
        MenuPosition = percent * GameManager.Resolution.Y - GameManager.Resolution.Y;
        var backgroundColor = 0.75F * percent;

        GameManager.SpriteBatch.Draw(GameManager.Pixel,
            new Rectangle(Point.Zero, GameManager.Resolution),
            Color.Black * (float)backgroundColor);
        GameManager.SpriteBatch.Draw(Assets.Instance.InventoryUi,
            new Rectangle(new Point(-16, (int)MenuPosition - 16), GameManager.Resolution),
            Assets.Instance.InventoryUi.Bounds,
            Color.White);

        DrawEquipment();
        DrawItems();
        DrawSelectedItemName();

        if (Globals.Player.GetInfo().Inventory.GetItems().Count > 0)
        {
            DrawCursor();
        }
    }

    private void DrawEquipment()
    {
        const int statsHeaderX = 176;
        const int statsHeaderY = 60;
        const string statsHeader = "Stats & Equip.";

        var statsHeaderDimensions = Assets.Instance.Monogram24.MeasureString(statsHeader);

        GameManager.SpriteBatch.DrawString(Assets.Instance.Monogram24,
            statsHeader,
            new Vector2(statsHeaderX - statsHeaderDimensions.X / 2, (float)(MenuPosition + statsHeaderY)),
            Color.Black);

        const int weaponHeaderX = 86;
        const int weaponHeaderY = 154;
        const string weaponHeader = "Weapon:";

        GameManager.SpriteBatch.DrawString(Assets.Instance.Monogram24,
            weaponHeader,
            new Vector2(weaponHeaderX, (float)MenuPosition + weaponHeaderY),
            Color.Black);

        const int weaponNameX = 265;
        const int weaponNameY = 180;

        var weapon = Globals.Player.GetInfo().Equipment.Weapon;
        var weaponName = weapon != null ? weapon.GetName() : "None";
        var weaponNameDimensions = Assets.Instance.Monogram24.MeasureString(weaponName);

        GameManager.SpriteBatch.DrawString(Assets.Instance.Monogram24,
            weaponName,
            new Vector2(weaponNameX - weaponNameDimensions.X, (float)MenuPosition + weaponNameY),
            Color.Black);
    }

    private void DrawItems()
    {
        const int inventoryHeaderX = 560;
        const int inventoryHeaderY = 58;
        const string inventoryHeader = "Inventory";

        var inventoryHeaderDimensions = Assets.Instance.Monogram24.MeasureString(inventoryHeader);

        GameManager.SpriteBatch.DrawString(Assets.Instance.Monogram24,
            inventoryHeader,
            new Vector2(inventoryHeaderX - inventoryHeaderDimensions.X / 2, (float)(MenuPosition + inventoryHeaderY)),
            Color.Black);

        const int inventoryX = 416;
        const int inventoryY = 128;
        var inventory = Globals.Player.GetInfo().Inventory;

        var i = 0;
        foreach (var (_, inventoryItem) in inventory.GetItems())
        {
            var gridX = i % 5;
            var gridY = i / 5;

            var x = inventoryX + gridX * 64;
            var y = (int)MenuPosition + inventoryY + gridY * 64;

            GameManager.SpriteBatch.Draw(inventoryItem.Item.GetTileSet(),
                new Rectangle(new Point(x, y),
                    new Point(32, 32)),
                inventoryItem.Item.GetSourceRectangle(),
                Color.White);

            var qtyDrawingData = Assets.Instance.Monogram24.GetDrawingData(inventoryItem.Quantity.ToString());
            GameManager.SpriteBatch.DrawTextDrawingData(qtyDrawingData,
                (d) => new Vector2(x + 32 - d.X, y + 32 - d.Y),
                Color.White);

            i++;
        }
    }

    private void DrawCursor()
    {
        const int inventoryX = 416;
        const int inventoryY = 128;

        GameManager.SpriteBatch.Draw(Assets.Instance.WoodCursorSpriteSheet,
            new Rectangle(new Point(inventoryX + CursorGridPosition.X * 64,
                    (int)MenuPosition + inventoryY + CursorGridPosition.Y * 64),
                new Point(32, 32)),
            CursorAnimation.GetSourceRectangle(), Color.White);
    }

    private void DrawSelectedItemName()
    {
        var inventory = Globals.Player.GetInfo().Inventory;

        if (inventory.GetItems().Count == 0) return;
        
        var (_, inventoryItem) = inventory.GetItems().ElementAt(Cursor);
        var itemName = inventoryItem.Item.GetName();
        var nameData = Assets.Instance.Monogram24.GetDrawingData(itemName);

        GameManager.SpriteBatch.DrawTextDrawingData(nameData,
            new Vector2(560 - nameData.Dimensions.X / 2, (float)MenuPosition + 510),
            Color.Black);
    }

    public override void Update(GameTime gameTime)
    {
        TransitionTimer.Update(gameTime);
        CursorAnimation.Update(gameTime);

        if (MenuClosed)
        {
            if (TransitionTimer.IsDone())
            {
                GameManager.StateManager.PopState();
                Flags.IsPaused = false;
            }

            return;
        }

        if (InputManager.Instance.IsKeyPressed(InputAction.ExitMenu))
        {
            MenuClosed = true;
            TransitionTimer.SetReverse();
        }

        if (InputManager.Instance.IsKeyPressed(InputAction.MoveSouth))
        {
            Cursor = CustomMath.WrapAround(Cursor + NumberOfColumns, 0,
                Globals.Player.GetInfo().Inventory.GetItems().Count - 1);
        }
        else if (InputManager.Instance.IsKeyPressed(InputAction.MoveNorth))
        {
            Cursor = CustomMath.WrapAround(Cursor - NumberOfColumns, 0,
                Globals.Player.GetInfo().Inventory.GetItems().Count - 1);
        }
        else if (InputManager.Instance.IsKeyPressed(InputAction.MoveEast))
        {
            Cursor = CustomMath.WrapAround(Cursor + 1, 0, Globals.Player.GetInfo().Inventory.GetItems().Count - 1);
        }
        else if (InputManager.Instance.IsKeyPressed(InputAction.MoveWest))
        {
            Cursor = CustomMath.WrapAround(Cursor - 1, 0, Globals.Player.GetInfo().Inventory.GetItems().Count - 1);
        }
    }
}