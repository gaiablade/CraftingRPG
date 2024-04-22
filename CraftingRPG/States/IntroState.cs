using CraftingRPG.Entities;
using CraftingRPG.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace CraftingRPG.States;

public class IntroState : IState
{
    private List<string> IntroStoryLines;

    public IntroState()
    {
        IntroStoryLines = GetStoryLines();
        if (!GameManager.FramesKeysHeld.ContainsKey(Keys.Enter))
        {
            GameManager.FramesKeysHeld.Add(Keys.Enter, 0);
        }
    }

    private List<string> GetStoryLines()
    {
        var storyLines = new List<string>();

        var introStory = "Welcome to Crafting RPG! While there is currently not much of a story, there are goals to complete " +
            "and fun to be had! The main goal of the game is to fight enemies, unlock recipes, gather materials, and craft " +
            "new weapons, armor, equipment, and items!";

        var allWords = introStory.Split(' ');
        var index = 0;
        var numberOfWords = 1;

        while (index < allWords.Length)
        {
            if (index + numberOfWords - 1 < allWords.Length && GameManager.DefaultFont.MeasureString(string.Join(' ', allWords.Skip(index).Take(numberOfWords))).X < GameManager.Resolution.X - 50)
            {
                numberOfWords++;
            }
            else
            {
                storyLines.Add(string.Join(' ', allWords.Skip(index).Take(numberOfWords - 1)));
                index = index + numberOfWords - 1;
                numberOfWords = 1;
            }
        }

        return storyLines;
    }

    public void Render()
    {
        var lineHeight = GameManager.DefaultFont.MeasureString(IntroStoryLines[0]).Y;
        var i = 0;
        var lastY = 0F;
        foreach (var line in IntroStoryLines)
        {
            var lineSize = GameManager.DefaultFont.MeasureString(line);
            lastY = 20 + (lineHeight + 5) * i;
            GameManager.SpriteBatch.DrawString(GameManager.DefaultFont,
                line,
                new Vector2(GameManager.Resolution.X / 2 - lineSize.X / 2, lastY),
                Color.White);
            i++;
        }

        var pressEnter = "Press Enter to Continue.";
        var pressEnterSize = GameManager.DefaultFont.MeasureString(pressEnter);
        GameManager.SpriteBatch.DrawString(GameManager.DefaultFont,
            pressEnter,
            new Vector2(GameManager.Resolution.X / 2 - pressEnterSize.X / 2, lastY + (GameManager.Resolution.Y - lastY) / 2 - pressEnterSize.Y / 2),
            Color.Orange);
    }

    public void Update(GameTime gameTime)
    {
        if (GameManager.FramesKeysHeld[Keys.Enter] == 1)
        {
            StateManager.Instance.PushState<OverworldState>();
        }
    }
}
