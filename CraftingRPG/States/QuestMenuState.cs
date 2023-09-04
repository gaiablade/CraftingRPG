using CraftingRPG.Interfaces;
using CraftingRPG.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

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
        HeaderSize = GameManager.Fnt20.MeasureString(Header);
        GameManager.SpriteBatch.DrawString(GameManager.Fnt20,
            Header,
            new Vector2(GameManager.Resolution.X / 2 - HeaderSize.X / 2, 10),
            Color.White);

        for (var i = 0; i < quests.Count; i++)
        {
            var text = $"{i + 1}) {quests[i].GetQuest().GetName()}";
            var textSize = GameManager.Fnt12.MeasureString(text);
            var color = Cursor == i ? Color.Orange : Color.White;
            GameManager.SpriteBatch.DrawString(GameManager.Fnt12,
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
        var color = questStatus == "COMPLETE" ? Color.Green : Color.Yellow;
        var questStatusSize = GameManager.Fnt12.MeasureString(questStatus);
        GameManager.SpriteBatch.DrawString(GameManager.Fnt12,
            questStatus,
            new Vector2(GameManager.Resolution.X / 2 + 25, 10 + HeaderSize.Y + 10),
            color);

        var allWords = description.Split(' ');
        var index = 0;
        var numberOfWords = 1;

        while (index < allWords.Length)
        {
            if (index + numberOfWords - 1 < allWords.Length && GameManager.Fnt12.MeasureString(string.Join(' ', allWords.Skip(index).Take(numberOfWords))).X < GameManager.Resolution.X / 2 - 50)
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

        var lineHeight = GameManager.Fnt12.MeasureString("A").Y;
        for (var i = 0; i < descLines.Count; i++)
        {
            GameManager.SpriteBatch.DrawString(GameManager.Fnt12,
                descLines[i],
                new Vector2(GameManager.Resolution.X / 2 + 25, 10 + HeaderSize.Y + 10 + questStatusSize.Y + 10 + (10 + lineHeight) * i),
                Color.White);
        }
    }

    public void Update()
    {
        if (GameManager.FramesKeysHeld[Keys.Up] == 1)
        {
            Cursor = CustomMath.WrapAround(Cursor - 1, 0, GameManager.PlayerInfo.Quests.Count - 1);
        } 
        else if (GameManager.FramesKeysHeld[Keys.Down] == 1)
        {
            Cursor = CustomMath.WrapAround(Cursor + 1, 0, GameManager.PlayerInfo.Quests.Count - 1);
        }

        if (GameManager.FramesKeysHeld[Keys.LeftControl] == 1)
        {
            GameManager.StateManager.PopState();
        }
    }
}
