﻿using CraftingRPG.Entities;
using CraftingRPG.Interfaces;
using CraftingRPG.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using CraftingRPG.Global;

namespace CraftingRPG.States;

public class QuestMenuState : IState
{
    private int Cursor = 0;
    private string Header = "Quests";
    private Vector2 HeaderSize;

    public QuestMenuState()
    {
        GameManager.AddKeysIfNotExists(Keys.LeftControl, Keys.Up, Keys.Down);
    }

    public void Render()
    {
        var quests = GameManager.PlayerInfo.Quests;

        Header = "Quests";
        HeaderSize = Globals.Instance.Fnt20.MeasureString(Header);
        GameManager.SpriteBatch.DrawString(Globals.Instance.Fnt20,
            Header,
            new Vector2(GameManager.Resolution.X / 2 - HeaderSize.X / 2, 10),
            Color.White);

        for (var i = 0; i < quests.Count; i++)
        {
            var text = $"{i + 1}) {quests[i].GetQuest().GetName()}";
            var textSize = Globals.Instance.Fnt12.MeasureString(text);
            var color = Cursor == i ? Color.Orange : Color.White;
            GameManager.SpriteBatch.DrawString(Globals.Instance.Fnt12,
                text,
                new Vector2(10, 10 + HeaderSize.Y + 10 + (10 + textSize.Y) * i),
                color);
        }

        DrawHighlightedQuestInfo();
    }

    private void DrawHighlightedQuestInfo()
    {
        var questInstance = GameManager.PlayerInfo.Quests[Cursor];
        var name = questInstance.GetQuest().GetName();
        var description = questInstance.GetQuest().GetDescription();
        var descLines = new List<string>();

        var questStatus = questInstance.IsComplete() ? "COMPLETE" : "IN PROGRESS";
        var color = questStatus == "COMPLETE" ? Color.LightGreen : Color.Yellow;
        var questStatusSize = Globals.Instance.Fnt12.MeasureString(questStatus);
        GameManager.SpriteBatch.DrawString(Globals.Instance.Fnt12,
            questStatus,
            new Vector2(GameManager.Resolution.X / 2 + 25, 10 + HeaderSize.Y + 10),
            color);

        var allWords = description.Split(' ');
        var index = 0;
        var numberOfWords = 1;

        while (index < allWords.Length)
        {
            if (index + numberOfWords - 1 < allWords.Length && Globals.Instance.Fnt12.MeasureString(string.Join(' ', allWords.Skip(index).Take(numberOfWords))).X < GameManager.Resolution.X / 2 - 50)
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

        var lineHeight = Globals.Instance.Fnt12.MeasureString("A").Y;
        for (var i = 0; i < descLines.Count; i++)
        {
            GameManager.SpriteBatch.DrawString(Globals.Instance.Fnt12,
                descLines[i],
                new Vector2((int)(GameManager.Resolution.X / 2 + 25), (int)(10 + HeaderSize.Y + 10 + questStatusSize.Y + 10 + (10 + lineHeight) * i)),
                Color.White);
        }

        var y = 10 + HeaderSize.Y + 10 + questStatusSize.Y + 10 + (10 + lineHeight) * descLines.Count + 10;

        var details = "Details";
        var detailsSize = Globals.Instance.Fnt12.MeasureString(details);
        GameManager.SpriteBatch.DrawString(Globals.Instance.Fnt12,
            details,
            new Vector2((int)(GameManager.Resolution.X / 2 + GameManager.Resolution.X / 4 - detailsSize.X / 2), y),
            Color.White);
        y += detailsSize.Y + 10;

        if (GameManager.PlayerInfo.Quests[Cursor] is FetchQuestInstance fetchQuest)
        {
            foreach (var (itemId, qty) in fetchQuest.GetCollectedItems())
            {
                var itemInfo = GameManager.ItemInfo[itemId];
                GameManager.SpriteBatch.Draw(GameManager.SpriteSheet,
                    new Rectangle(GameManager.Resolution.X / 2 + 10, (int)y, 32, 32),
                    new Rectangle(0, 32 * itemInfo.GetSpriteSheetIndex(), 32, 32),
                    Color.White);
                var itemName = itemInfo.GetName();
                var quest = fetchQuest.GetFetchQuest();
                var requestedQty = quest.GetRequiredItems()[itemId];
                itemName += $" {qty}/{requestedQty}";
                var itemNameSize = Globals.Instance.Fnt12.MeasureString(itemName);
                color = qty >= requestedQty ? Color.LightGreen : Color.White;
                GameManager.SpriteBatch.DrawString(Globals.Instance.Fnt12,
                    itemName,
                    new Vector2(GameManager.Resolution.X / 2 + 10 + 32 + 5, (int)y + 16 - itemNameSize.Y / 2),
                    color);
                y += 32 + 10;
            }
        }
    }

    public void Update(GameTime gameTime)
    {
        if (GameManager.FramesKeysHeld[Keys.Up] == 1)
        {
            GameManager.MenuHoverSfx01.Play(0.3F, 0F, 0F);
            Cursor = CustomMath.WrapAround(Cursor - 1, 0, GameManager.PlayerInfo.Quests.Count - 1);
        } 
        else if (GameManager.FramesKeysHeld[Keys.Down] == 1)
        {
            GameManager.MenuHoverSfx01.Play(0.3F, 0F, 0F);
            Cursor = CustomMath.WrapAround(Cursor + 1, 0, GameManager.PlayerInfo.Quests.Count - 1);
        }

        if (GameManager.FramesKeysHeld[Keys.LeftControl] == 1)
        {
            GameManager.StateManager.PopState();
        }
    }
}
