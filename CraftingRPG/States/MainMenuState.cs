﻿using CraftingRPG.AssetManagement;
using CraftingRPG.Enums;
using CraftingRPG.GameStateManagement;
using CraftingRPG.InputManagement;
using CraftingRPG.Interfaces;
using Microsoft.Xna.Framework;

namespace CraftingRPG.States;

public class MainMenuState : IState
{
    public MainMenuState()
    {
    }

    public void DrawWorld()
    {
        var gameTitle = "Crafting RPG";
        var titleSize = Assets.Instance.Monogram24.MeasureString(gameTitle);
        GameManager.SpriteBatch.DrawString(Assets.Instance.Monogram24,
            gameTitle,
            new Vector2(GameManager.Resolution.X / 2 - titleSize.X / 2, 50),
            Color.White);

        var pressEnter = "Press Enter to Start!";
        var pressEnterSize = Assets.Instance.Monogram24.MeasureString(pressEnter);
        GameManager.SpriteBatch.DrawString(Assets.Instance.Monogram24,
            pressEnter,
            new Vector2(GameManager.Resolution.X / 2 - pressEnterSize.X / 2, GameManager.Resolution.Y / 2 - pressEnterSize.Y / 2),
            Color.Orange);
    }

    public void DrawUI()
    {
    }

    public void Update(GameTime gameTime)
    {
        if (InputManager.Instance.GetKeyPressState(InputAction.MenuSelect) == KeyPressState.Pressed)
        {
            GameStateManager.Instance.PushState<IntroState>();
        }
    }
}
