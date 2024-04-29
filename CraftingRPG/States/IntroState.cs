using CraftingRPG.Interfaces;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using CraftingRPG.AssetManagement;
using CraftingRPG.Enums;
using CraftingRPG.GameStateManagement;
using CraftingRPG.InputManagement;

namespace CraftingRPG.States;

public class IntroState : IState
{
    private List<string> IntroStoryLines;

    public IntroState()
    {
        IntroStoryLines = GetStoryLines();
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
            if (index + numberOfWords - 1 < allWords.Length && Assets.Instance.Monogram24.MeasureString(string.Join(' ', allWords.Skip(index).Take(numberOfWords))).X < GameManager.Resolution.X - 50)
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

    public void DrawWorld()
    {
        var lineHeight = Assets.Instance.Monogram24.MeasureString(IntroStoryLines[0]).Y;
        var i = 0;
        var lastY = 0F;
        foreach (var line in IntroStoryLines)
        {
            var lineSize = Assets.Instance.Monogram24.MeasureString(line);
            lastY = 20 + (lineHeight + 5) * i;
            GameManager.SpriteBatch.DrawString(Assets.Instance.Monogram24,
                line,
                new Vector2(GameManager.Resolution.X / 2 - lineSize.X / 2, lastY),
                Color.White);
            i++;
        }

        var pressEnter = "Press Enter to Continue.";
        var pressEnterSize = Assets.Instance.Monogram24.MeasureString(pressEnter);
        GameManager.SpriteBatch.DrawString(Assets.Instance.Monogram24,
            pressEnter,
            new Vector2(GameManager.Resolution.X / 2 - pressEnterSize.X / 2, lastY + (GameManager.Resolution.Y - lastY) / 2 - pressEnterSize.Y / 2),
            Color.Orange);
    }

    public void DrawUI()
    {
    }

    public void Update(GameTime gameTime)
    {
        if (InputManager.Instance.IsKeyPressed(InputAction.MenuSelect))
        {
            GameStateManager.Instance.PushState<OverworldState>();
        }
    }
}
