using CraftingRPG.AssetManagement;
using CraftingRPG.Enums;
using CraftingRPG.Extensions;
using CraftingRPG.InputManagement;
using CraftingRPG.Interfaces;
using CraftingRPG.Lerpers;
using CraftingRPG.SourceRectangleProviders;
using Microsoft.Xna.Framework;

namespace CraftingRPG.GameStateManagement.GameStates;

public class MainMenuGameState : BaseGameState
{
    private ISourceRectangleProvider<InputAction> SourceRectangleProvider;
    private MainMenuState State = MainMenuState.Normal;
    
    // Fade out
    private ILerper<float> FadeOutLerper;

    public MainMenuGameState()
    {
        SourceRectangleProvider = new InputActionKeySourceRectangleProvider();
        FadeOutLerper = new LinearFloatLerper(0F, 1F, 1);
    }
    
    public override void DrawWorld()
    {
    }

    public override void DrawUi()
    {
        GameManager.SpriteBatch.Draw(Assets.Instance.TitleUi,
            GameManager.WindowBounds,
            Color.White);
        
        // Get key
        var sourceRectangle = SourceRectangleProvider.GetSourceRectangle(InputAction.MenuSelect);
        var startTextData = Assets.Instance.Monogram24.GetDrawingData(" START");
        var totalWidth = sourceRectangle.Width + startTextData.Dimensions.X;
        var x = GameManager.WindowBounds.Center.X - totalWidth / 2;
        
        GameManager.SpriteBatch.Draw(Assets.Instance.KeyIconSpriteSheet,
            new Vector2(x, 400),
            sourceRectangle,
            Color.White);
        GameManager.SpriteBatch.DrawTextDrawingData(startTextData,
            new Vector2(x + sourceRectangle.Width, 400),
            Color.Black);
        
        GameManager.SpriteBatch.Draw(GameManager.Pixel,
            GameManager.WindowBounds,
            Color.Black * FadeOutLerper.GetLerpedValue());
    }

    public override void Update(GameTime gameTime)
    {
        if (State == MainMenuState.Normal)
        {
            if (InputManager.Instance.GetKeyPressState(InputAction.MenuSelect) == KeyPressState.Pressed)
            {
                State = MainMenuState.FadingOut;
            }
        }
        else if (State == MainMenuState.FadingOut)
        {
            FadeOutLerper.Update(gameTime);
            if (FadeOutLerper.IsDone())
            {
                GameStateManager.Instance.PushState<InstructionsGameState>();
            }
        }
    }

    private void SetState(MainMenuState state)
    {
        State = state;
        switch (state)
        {
            case MainMenuState.FadingOut:
                break;
        }
    }

    private enum MainMenuState
    {
        Normal,
        FadingOut
    }
}
