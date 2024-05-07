using CraftingRPG.AssetManagement;
using CraftingRPG.Global;
using CraftingRPG.Interfaces;
using CraftingRPG.Lerpers;
using CraftingRPG.SoundManagement;
using Microsoft.Xna.Framework;

namespace CraftingRPG.GameStateManagement.GameStates;

public class DemoEndingGameState : BaseGameState
{
    private ILerper<float> FadeOutLerper;
    private ILerper<float> FadeInLerper;
    private DemoEndingState State = DemoEndingState.FadingOut;

    public DemoEndingGameState()
    {
        SoundManager.Instance.FadeOut(2);
        Flags.IsPaused = true;
        FadeOutLerper = new LinearFloatLerper(0F, 1F, 2);
        FadeInLerper = new LinearFloatLerper(1F, 0F, 2);
    }

    public override void Update(GameTime gameTime)
    {
        if (State == DemoEndingState.FadingOut)
        {
            FadeOutLerper.Update(gameTime);
            if (FadeOutLerper.IsDone())
            {
                State = DemoEndingState.FadingIn;
            }
        }
        else if (State == DemoEndingState.FadingIn)
        {
            FadeInLerper.Update(gameTime);
        }
    }

    public override void DrawUi()
    {
        if (State == DemoEndingState.FadingOut)
        {
            GameManager.SpriteBatch.Draw(GameManager.Pixel,
                GameManager.WindowBounds,
                Color.Black * FadeOutLerper.GetLerpedValue());
        }
        else
        {
            GameManager.SpriteBatch.Draw(Assets.Instance.ThanksForPlaying,
                GameManager.WindowBounds,
                Color.White);
            GameManager.SpriteBatch.Draw(GameManager.Pixel,
                GameManager.WindowBounds,
                Color.Black * FadeInLerper.GetLerpedValue());
        }
    }

    private enum DemoEndingState
    {
        FadingIn,
        FadingOut
    }
}