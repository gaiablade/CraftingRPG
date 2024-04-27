using CraftingRPG.Entities;
using CraftingRPG.Global;
using CraftingRPG.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CraftingRPG.States;

public class MainMenuState : IState
{
    public MainMenuState()
    {
        if (GameManager.FramesKeysHeld.ContainsKey(Keys.Enter))
        {
            GameManager.FramesKeysHeld[Keys.Enter] = 0;
        }
        else
        {
            GameManager.FramesKeysHeld.Add(Keys.Enter, 0);
        }
    }

    public void DrawWorld()
    {
        var gameTitle = "Crafting RPG";
        var titleSize = Globals.Instance.DefaultFont.MeasureString(gameTitle);
        GameManager.SpriteBatch.DrawString(Globals.Instance.DefaultFont,
            gameTitle,
            new Vector2(GameManager.Resolution.X / 2 - titleSize.X / 2, 50),
            Color.White);

        var pressEnter = "Press Enter to Start!";
        var pressEnterSize = Globals.Instance.DefaultFont.MeasureString(pressEnter);
        GameManager.SpriteBatch.DrawString(Globals.Instance.DefaultFont,
            pressEnter,
            new Vector2(GameManager.Resolution.X / 2 - pressEnterSize.X / 2, GameManager.Resolution.Y / 2 - pressEnterSize.Y / 2),
            Color.Orange);
    }

    public void DrawUI()
    {
    }

    public void Update(GameTime gameTime)
    {
        if (GameManager.FramesKeysHeld[Keys.Enter] == 1)
        {
            StateManager.Instance.PushState<IntroState>();
        }
    }
}
