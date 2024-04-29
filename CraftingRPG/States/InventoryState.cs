using CraftingRPG.Interfaces;
using CraftingRPG.Utility;
using Microsoft.Xna.Framework;
using System.Linq;
using CraftingRPG.AssetManagement;
using CraftingRPG.Enums;
using CraftingRPG.Global;
using CraftingRPG.InputManagement;
using CraftingRPG.SpriteAnimation;
using CraftingRPG.Timers;

namespace CraftingRPG.States;

public class InventoryState : IState
{
    private const int NumberOfColumns = 5;

    private int ActiveTab = 0;
    private Point Cursor = new(0, 0);

    // Drawing constants
    private readonly int CenterX = GameManager.Resolution.X / 2;
    private readonly int ItemWidthAndGap = 32 + 30;
    private readonly int ItemHeightAndGap = 32 + 10;
    private readonly int GridTop = 150;

    private ITimer TransitionTimer;
    private double MenuPosition;
    private bool MenuClosed;

    private Animation CursorAnimation;

    public InventoryState()
    {
        TransitionTimer = new EaseOutTimer(0.5);
        CursorAnimation = new Animation(4, 0.4, new Point(32, 32));
    }

    public void DrawWorld()
    {
    }

    public void DrawUI()
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

        if (Globals.Player.Info.Inventory.Items.Count > 0)
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

        var weapon = Globals.Player.Info.Equipment.Weapon;
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
        var inventory = Globals.Player.Info.Inventory;

        var i = 0;
        foreach (var (itemId, quantity) in inventory.Items)
        {
            var itemInfo = GameManager.ItemInfo[itemId];

            var gridX = i % 5;
            var gridY = i / 5;

            GameManager.SpriteBatch.Draw(itemInfo.GetTileSet(),
                new Rectangle(new Point(inventoryX + gridX * 64, (int)MenuPosition + inventoryY + gridY * 64),
                    new Point(32, 32)),
                itemInfo.GetSourceRectangle(),
                Color.White);

            i++;
        }
    }

    public void DrawCursor()
    {
        const int inventoryX = 416;
        const int inventoryY = 128;

        GameManager.SpriteBatch.Draw(Assets.Instance.WoodCursorSpriteSheet,
            new Rectangle(new Point(inventoryX + Cursor.X * 64, 
                    (int)MenuPosition + inventoryY + Cursor.Y * 64), 
                new Point(32, 32)),
            CursorAnimation.GetSourceRectangle(), Color.White);
    }

    public void DrawSelectedItemName()
    {
        var inventory = Globals.Player.Info.Inventory;
        var cursorFlat = Cursor.Y * NumberOfColumns + Cursor.X;
        var (itemId, qty) = inventory.Items.ElementAt(cursorFlat);
        var itemInfo = GameManager.ItemInfo[itemId];
        var itemName = itemInfo.GetName() + " x" + qty;
        var itemNameSize = Assets.Instance.Monogram24.MeasureString(itemName);
        GameManager.SpriteBatch.DrawString(Assets.Instance.Monogram24,
            itemName,
            new Vector2(CenterX + CenterX / 2 - itemNameSize.X / 2, 100),
            Color.White);
    }

    public void Update(GameTime gameTime)
    {
        TransitionTimer.Update(gameTime);
        CursorAnimation.Update(gameTime);

        if (MenuClosed)
        {
            if (TransitionTimer.IsDone())
            {
                GameManager.StateManager.PopState();
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
            Cursor.X = CustomMath.WrapAround(Cursor.X - 1, 0, NumberOfColumns - 1);
        }
        else if (InputManager.Instance.IsKeyPressed(InputAction.MoveEast))
        {
            Cursor.X = CustomMath.WrapAround(Cursor.X + 1, 0, NumberOfColumns - 1);
        }
        else if (InputManager.Instance.IsKeyPressed(InputAction.MoveSouth))
        {
            Cursor.Y++;
        }
        else if (InputManager.Instance.IsKeyPressed(InputAction.MoveNorth))
        {
            Cursor.Y--;
        }

        NormalizeCursor();
    }

    private void NormalizeCursor()
    {
        var inventory = Globals.Player.Info.Inventory;
        var numRows = inventory.Items.Count / NumberOfColumns + 1;
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
            var lastRowColumns = inventory.Items.Count % NumberOfColumns;
            if (Cursor.X > lastRowColumns - 1)
            {
                Cursor.X = lastRowColumns - 1;
            }
        }
    }
}