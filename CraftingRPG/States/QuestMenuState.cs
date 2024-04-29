using System;
using CraftingRPG.Interfaces;
using CraftingRPG.Utility;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using CraftingRPG.AssetManagement;
using CraftingRPG.Enums;
using CraftingRPG.InputManagement;
using CraftingRPG.Quests;
using CraftingRPG.Timers;

namespace CraftingRPG.States;

public class QuestMenuState : IState
{
    private int Cursor = 0;
    private string Header = "Quests";
    private Vector2 HeaderSize;

    private ITimer TransitionTimer;
    private double MenuPosition;
    private bool MenuClosed;

    public QuestMenuState()
    {
        TransitionTimer = new EaseOutTimer(0.4);
    }

    public void DrawWorld()
    {
    }

    public void DrawUI()
    {
        var percent = (float)TransitionTimer.GetPercent();
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

        const int questNoteX = 96;
        const int questNoteY = 112;
        const int questNameX = 119;
        const int questNameY = 135;
        const int maxCharsPerLine = 13;

        var i = 0;
        var quests = GameManager.PlayerInfo.Quests;
        foreach (var questInstance in quests)
        {
            var questName = questInstance.GetQuest().GetName();
            var lines = BreakUpString(questName, maxCharsPerLine);
            var questNameDimensions = Assets.Instance.Monogram24.MeasureString(questName);

            GameManager.SpriteBatch.Draw(Assets.Instance.PaperNoteSpriteSheet,
                new Rectangle(questNoteX, (int)MenuPosition + questNoteY, 192, 192),
                new Rectangle(288, 32, 96, 96),
                Color.White);

            var j = 0;
            foreach (var line in lines)
            {
                GameManager.SpriteBatch.DrawString(Assets.Instance.Monogram24,
                    line,
                    new Vector2(questNameX, (int)MenuPosition + questNameY + questNameDimensions.Y * j),
                    Color.Black);
                j++;
            }
            i++;
        }
    }

    private void DrawHighlightedQuestInfo()
    {
        var questInstance = GameManager.PlayerInfo.Quests[Cursor];
        var name = questInstance.GetQuest().GetName();
        var description = questInstance.GetQuest().GetDescription();
        var descLines = new List<string>();

        var questStatus = questInstance.IsComplete() ? "COMPLETE" : "IN PROGRESS";
        var color = questStatus == "COMPLETE" ? Color.LightGreen : Color.Yellow;
        var questStatusSize = Assets.Instance.Monogram24.MeasureString(questStatus);
        GameManager.SpriteBatch.DrawString(Assets.Instance.Monogram24,
            questStatus,
            new Vector2(GameManager.Resolution.X / 2 + 25, 10 + HeaderSize.Y + 10),
            color);

        var allWords = description.Split(' ');
        var index = 0;
        var numberOfWords = 1;

        while (index < allWords.Length)
        {
            if (index + numberOfWords - 1 < allWords.Length &&
                Assets.Instance.Monogram24.MeasureString(string.Join(' ', allWords.Skip(index).Take(numberOfWords))).X <
                GameManager.Resolution.X / 2 - 50)
            {
                numberOfWords++;
            }
            else
            {
                descLines.Add(string.Join(' ', allWords.Skip(index).Take(numberOfWords - 1)));
                index = index + numberOfWords - 1;
                numberOfWords = 1;
            }
        }

        var lineHeight = Assets.Instance.Monogram24.MeasureString("A").Y;
        for (var i = 0; i < descLines.Count; i++)
        {
            GameManager.SpriteBatch.DrawString(Assets.Instance.Monogram24,
                descLines[i],
                new Vector2((int)(GameManager.Resolution.X / 2 + 25),
                    (int)(10 + HeaderSize.Y + 10 + questStatusSize.Y + 10 + (10 + lineHeight) * i)),
                Color.White);
        }

        var y = 10 + HeaderSize.Y + 10 + questStatusSize.Y + 10 + (10 + lineHeight) * descLines.Count + 10;

        var details = "Details";
        var detailsSize = Assets.Instance.Monogram24.MeasureString(details);
        GameManager.SpriteBatch.DrawString(Assets.Instance.Monogram24,
            details,
            new Vector2((int)(GameManager.Resolution.X / 2 + GameManager.Resolution.X / 4 - detailsSize.X / 2), y),
            Color.White);
        y += detailsSize.Y + 10;

        if (GameManager.PlayerInfo.Quests[Cursor] is FetchQuestInstance fetchQuest)
        {
            foreach (var (itemId, qty) in fetchQuest.GetCollectedItems())
            {
                var itemInfo = GameManager.ItemInfo[itemId];
                GameManager.SpriteBatch.Draw(itemInfo.GetTileSet(),
                    new Rectangle(GameManager.Resolution.X / 2 + 10, (int)y, 32, 32),
                    itemInfo.GetSourceRectangle(),
                    Color.White);
                var itemName = itemInfo.GetName();
                var quest = fetchQuest.GetFetchQuest();
                var requestedQty = quest.GetRequiredItems()[itemId];
                itemName += $" {qty}/{requestedQty}";
                var itemNameSize = Assets.Instance.Monogram24.MeasureString(itemName);
                color = qty >= requestedQty ? Color.LightGreen : Color.White;
                GameManager.SpriteBatch.DrawString(Assets.Instance.Monogram24,
                    itemName,
                    new Vector2(GameManager.Resolution.X / 2 + 10 + 32 + 5, (int)y + 16 - itemNameSize.Y / 2),
                    color);
                y += 32 + 10;
            }
        }
    }

    public void Update(GameTime gameTime)
    {
        TransitionTimer.Update(gameTime);

        if (MenuClosed)
        {
            if (TransitionTimer.IsDone())
            {
                GameManager.StateManager.PopState();
            }

            return;
        }

        if (InputManager.Instance.IsKeyPressed(InputAction.MoveNorth))
        {
            GameManager.MenuHoverSfx01.Play(0.3F, 0F, 0F);
            Cursor = CustomMath.WrapAround(Cursor - 1, 0, GameManager.PlayerInfo.Quests.Count - 1);
        }
        else if (InputManager.Instance.IsKeyPressed(InputAction.MoveSouth))
        {
            GameManager.MenuHoverSfx01.Play(0.3F, 0F, 0F);
            Cursor = CustomMath.WrapAround(Cursor + 1, 0, GameManager.PlayerInfo.Quests.Count - 1);
        }

        if (InputManager.Instance.IsKeyPressed(InputAction.ExitMenu))
        {
            TransitionTimer.SetReverse();
            MenuClosed = true;
        }
    }

    private string[] BreakUpString(string s, int charsPerLine)
    {
        var list = new List<string>();
        var currentIndex = 0;
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
}