using System;
using System.Collections.Generic;
using System.Linq;
using CraftingRPG.AssetManagement;
using CraftingRPG.Enums;
using CraftingRPG.Extensions;
using CraftingRPG.Global;
using CraftingRPG.InputManagement;
using CraftingRPG.Interfaces;
using CraftingRPG.QuestManagement;
using CraftingRPG.Timers;
using CraftingRPG.Utility;
using Microsoft.Xna.Framework;

namespace CraftingRPG.GameStateManagement.GameStates;

public class QuestMenuGameState : BaseGameState
{
    // Menu Transition
    private readonly ITimer MenuTransitionTimer;
    private double MenuPosition;
    private bool MenuClosed;

    // Cursor Sprite
    private readonly Rectangle CursorSprite = new(128, 160, 32, 32);
    private readonly ITimer CursorHoverTimer;
    private readonly ITimer CursorMovementTimer;
    private Vector2 CursorPosition;
    private int Cursor;

    // Menu State
    private const int QuestMenu = 0;
    private const int QuestDetailsMenu = 1;
    private int MenuState = QuestMenu;

    // Constants
    private const int QuestNoteX = 84;
    private const int QuestNoteY = 112;

    public QuestMenuGameState()
    {
        MenuTransitionTimer = new EaseOutTimer(0.4);
        CursorHoverTimer = new EaseInOutTimer(1.0);
        CursorMovementTimer = new EaseOutTimer(0.4);
        CursorMovementTimer.MaxOut();
    }

    public override void DrawUi()
    {
        var percent = (float)MenuTransitionTimer.GetPercent();
        MenuPosition = percent * GameManager.Resolution.Y - GameManager.Resolution.Y;
        var backgroundColor = Color.Black * 0.75F * percent;

        GameManager.SpriteBatch.Draw(GameManager.Pixel,
            new Rectangle(Point.Zero, GameManager.Resolution),
            backgroundColor);
        GameManager.SpriteBatch.Draw(Assets.Instance.QuestUi,
            new Rectangle(new Point(-16, (int)MenuPosition - 16), GameManager.Resolution),
            Assets.Instance.QuestUi.Bounds,
            Color.White);

        const int questHeaderX = 400;
        const int questHeaderY = 57;
        const string questHeader = "Quests";
        var questHeaderDimensions = Assets.Instance.Monogram24.MeasureString(questHeader);

        GameManager.SpriteBatch.DrawString(Assets.Instance.Monogram24,
            questHeader,
            new Vector2(questHeaderX - questHeaderDimensions.X / 2, (int)MenuPosition + questHeaderY),
            Color.Black);
        
        const int maxCharsPerLine = 13;

        var questBook = Globals.PlayerInfo.QuestBook;
        foreach (var (questInstance, i) in questBook.GetActiveQuests().WithIndex())
        {
            var x = i % 3;
            var y = i / 3;
            var questPosition = new Vector2(QuestNoteX + 220 * x, (float)MenuPosition + QuestNoteY + 200 * y);

            var questName = questInstance.GetQuestInfo().GetName();
            var lines = BreakUpString(questName, maxCharsPerLine);
            var questNameDimensions = Assets.Instance.Monogram24.MeasureString(questName);
            var noteSize = new Point(192, 192);
            var notePosition = questPosition.ToPoint();

            GameManager.SpriteBatch.Draw(Assets.Instance.PaperNoteSpriteSheet,
                new Rectangle(notePosition, noteSize),
                new Rectangle(288, 32, 96, 96),
                Color.White);

            var questNamePosition = new Vector2(questPosition.X + 35, questPosition.Y + 23);
            foreach (var (line, j) in lines.WithIndex())
            {
                GameManager.SpriteBatch.DrawString(Assets.Instance.Monogram24,
                    line,
                    new Vector2(questNamePosition.X, questNamePosition.Y + questNameDimensions.Y * j),
                    Color.Black);
            }
        }
        
        var cursorHoverPercent = CursorHoverTimer.GetPercent();
        var cursorMovementPercent = CursorMovementTimer.GetPercent();
        var currentCursorPosition = GetCursorCurrentPosition();
        CursorPosition =
            Vector2.Lerp(CursorPosition, currentCursorPosition, (float)cursorMovementPercent);
        CursorPosition.X += (float)cursorHoverPercent * 5;
        CursorPosition.Y -= (float)cursorHoverPercent * 5;

        GameManager.SpriteBatch.Draw(Assets.Instance.IconSpriteSheet,
            new Rectangle(CursorPosition.ToPoint(), new Point(32, 32)),
            CursorSprite,
            Color.White);

        if (MenuState == QuestDetailsMenu)
        {
            var selectedQuest = Globals.Player.Info.QuestBook.GetActiveQuests()[Cursor];

            GameManager.SpriteBatch.Draw(Assets.Instance.QuestDetailsUi,
                new Rectangle(new Point(-16, -16), GameManager.Resolution),
                Assets.Instance.QuestDetailsUi.Bounds,
                Color.White);

            var nameData = Assets.Instance.Monogram24.GetDrawingData(selectedQuest.GetQuestInfo().GetName());
            var nameX = GameManager.Resolution.ToVector2().X / 2 - nameData.Dimensions.X / 2;

            GameManager.SpriteBatch.DrawTextDrawingData(nameData,
                new Vector2(nameX, 80),
                Color.Black);

            var descLines = BreakUpString(selectedQuest.GetQuestInfo().GetDescription(), 50);
            var descHeight = descLines.Sum(x => Assets.Instance.Monogram24.MeasureString(x).Y);
            var descLineHeight = descHeight / descLines.Length;
            var descX = 84;
            var descY = 144;

            foreach (var (line, idx) in descLines.WithIndex())
            {
                var lineY = descY + descLineHeight * idx;
                GameManager.SpriteBatch.DrawString(Assets.Instance.Monogram24,
                    line,
                    new Vector2(descX, lineY),
                    Color.Black);
            }

            var detailsY = (int)(descY + descHeight + 25);
            if (selectedQuest is CraftQuestInstance craftQuestInstance)
            {
                var requiredItemsToCraft = craftQuestInstance.GetCraftQuestInfo().GetRequiredItemsToCraft();
                foreach (var ((itemInfo, qty), i) in requiredItemsToCraft.WithIndex())
                {
                    GameManager.SpriteBatch.Draw(itemInfo.GetTileSet(),
                        new Rectangle(new Point(descX, detailsY + i * 32), new Point(32, 32)),
                        itemInfo.GetSourceRectangle(),
                        Color.White);

                    var itemDrawingData = Assets.Instance.Monogram24.GetDrawingData(itemInfo.GetName());
                    GameManager.SpriteBatch.DrawString(itemDrawingData.Font,
                        itemDrawingData.Message,
                        new Vector2(descX + 40, detailsY + i * 32),
                        Color.Black);

                    var itemAmount = craftQuestInstance.GetCraftedCount(itemInfo);
                    var requiredAmount = qty;
                    var quantityString = $"{itemAmount}/{requiredAmount}";
                    var quantityColor = itemAmount >= requiredAmount ? Color.DarkGreen : Color.DarkRed;

                    GameManager.SpriteBatch.DrawString(Assets.Instance.Monogram24,
                        quantityString,
                        new Vector2(descX + 60 + itemDrawingData.Dimensions.X, detailsY + i * 32),
                        quantityColor);
                }
            }
            else if (selectedQuest is FetchQuestInstance fetchQuestInstance)
            {
                var requiredItemsToGet = fetchQuestInstance.GetFetchQuestInfo().GetRequiredItems();
                foreach (var ((itemInfo, qty), i) in requiredItemsToGet.WithIndex())
                {
                    GameManager.SpriteBatch.Draw(itemInfo.GetTileSet(),
                        new Rectangle(new Point(descX, detailsY + i * 32), new Point(32, 32)),
                        itemInfo.GetSourceRectangle(),
                        Color.White);

                    var itemNameDimensions = Assets.Instance.Monogram24.MeasureString(itemInfo.GetName());
                    GameManager.SpriteBatch.DrawString(Assets.Instance.Monogram24,
                        itemInfo.GetName(),
                        new Vector2(descX + 40, detailsY + i * 32),
                        Color.Black);

                    var itemAmount = fetchQuestInstance.GetCollectedItems()[itemInfo];
                    var requiredAmount = qty;
                    var quantityString = $"{itemAmount}/{requiredAmount}";
                    var quantityColor = itemAmount >= requiredAmount ? Color.DarkGreen : Color.DarkRed;

                    GameManager.SpriteBatch.DrawString(Assets.Instance.Monogram24,
                        quantityString,
                        new Vector2(descX + 60 + itemNameDimensions.X, detailsY + i * 32),
                        quantityColor);
                }
            }
            else if (selectedQuest is DefeatEnemyQuestInstance defeatEnemyQuestInstance)
            {
                var questInfo = defeatEnemyQuestInstance.GetDefeatEnemyQuestInfo();
                var requiredEnemies = questInfo.GetRequiredEnemiesToDefeat();

                foreach (var ((enemyId, _), i) in requiredEnemies.WithIndex())
                {
                    var enemyInfo = EnemyInfo.Instance.GetEnemy(enemyId);

                    var enemyNameData = Assets.Instance.Monogram24.GetDrawingData(enemyInfo.GetName());
                    GameManager.SpriteBatch.DrawTextDrawingData(enemyNameData,
                        new Vector2(descX, detailsY + i * 32),
                        Color.Black);

                    var amountDefeated = defeatEnemyQuestInstance.GetDefeatedEnemyCount(enemyId);
                    var amountRequired = questInfo.GetRequiredEnemiesToDefeat()[enemyId];
                    var quantityData = Assets.Instance.Monogram24.GetDrawingData($"{amountDefeated}/{amountRequired}");
                    var quantityColor = amountDefeated >= amountRequired ? Color.DarkGreen : Color.DarkRed;

                    GameManager.SpriteBatch.DrawTextDrawingData(quantityData,
                        new Vector2(descX + enemyNameData.Dimensions.X + 20, detailsY + i * 32),
                        quantityColor);
                }
            }
        }
    }

    public override void Update(GameTime gameTime)
    {
        MenuTransitionTimer.Update(gameTime);
        CursorHoverTimer.Update(gameTime);
        CursorMovementTimer.Update(gameTime);

        if (CursorHoverTimer.IsDone())
        {
            if (CursorHoverTimer.GetReverse())
            {
                CursorHoverTimer.SetReverse(false);
            }
            else
            {
                CursorHoverTimer.SetReverse();
            }
        }

        if (MenuClosed)
        {
            if (MenuTransitionTimer.IsDone())
            {
                GameManager.StateManager.PopState();
            }

            return;
        }

        if (MenuState == QuestMenu)
        {
            if (InputManager.Instance.IsKeyPressed(InputAction.MoveWest))
            {
                Assets.Instance.MenuHoverSfx01.Play(0.3F, 0F, 0F);
                Cursor = CustomMath.WrapAround(Cursor - 1, 0,
                    Globals.PlayerInfo.QuestBook.GetActiveQuestCount() - 1);
                CursorMovementTimer.Reset();
            }
            else if (InputManager.Instance.IsKeyPressed(InputAction.MoveEast))
            {
                Assets.Instance.MenuHoverSfx01.Play(0.3F, 0F, 0F);
                Cursor = CustomMath.WrapAround(Cursor + 1, 0,
                    Globals.PlayerInfo.QuestBook.GetActiveQuestCount() - 1);
                CursorMovementTimer.Reset();
            }
            else if (InputManager.Instance.IsKeyPressed(InputAction.MenuSelect))
            {
                MenuState = QuestDetailsMenu;
            }
            else if (InputManager.Instance.IsKeyPressed(InputAction.ExitMenu))
            {
                MenuTransitionTimer.SetReverse();
                MenuClosed = true;
            }
        }
        else if (MenuState == QuestDetailsMenu)
        {
            if (InputManager.Instance.IsKeyPressed(InputAction.ExitMenu))
            {
                MenuState = QuestMenu;
            }
        }
    }

    private string[] BreakUpString(string s, int charsPerLine)
    {
        var list = new List<string>();
        int currentIndex;
        var lastWrap = 0;

        do
        {
            currentIndex = lastWrap + charsPerLine > s.Length
                ? s.Length
                : (s.LastIndexOfAny(new[] { ' ', ',', '.', '?', '!', ':', ';', '-', '\n', '\r', '\t' },
                    Math.Min(s.Length - 1, lastWrap + charsPerLine)) + 1);

            if (currentIndex <= lastWrap)
                currentIndex = Math.Min(lastWrap + charsPerLine, s.Length);

            list.Add(s.Substring(lastWrap, currentIndex - lastWrap).Trim(' '));
            lastWrap = currentIndex;
        } while (currentIndex < s.Length);

        return list.ToArray();
    }

    private Vector2 GetCursorCurrentPosition()
    {
        var x = Cursor % 3;
        var y = Cursor / 3;
        return new Vector2(QuestNoteX + 220 * x,
            (float)MenuPosition + QuestNoteY + 180 + 200 * y);
    }
}