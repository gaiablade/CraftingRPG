using CraftingRPG.AssetManagement;
using CraftingRPG.Enums;
using CraftingRPG.Extensions;
using CraftingRPG.InputManagement;
using CraftingRPG.Interfaces;
using CraftingRPG.Lerpers;
using CraftingRPG.Utility;
using Microsoft.Xna.Framework;

namespace CraftingRPG.GameStateManagement.GameStates;

public class InstructionsGameState : BaseGameState
{
    private const string InstructionsString
        = "Welcome to CraftingRPG! This is a short technical demo and has a few quick goals to complete. Feel free " +
          "to adjust the default keybindings in the keyconfig.xml file located in the same directory as the game. " +
          "Check the Quest menu to see what you need to do to complete the game!";

    private InstructionsState State = InstructionsState.FadingIn;
    private ILerper<float> FadeLerper;

    public InstructionsGameState()
    {
        FadeLerper = new LinearFloatLerper(1F, 0F, 1);
    }
    
    public override void DrawUi()
    {
        GameManager.SpriteBatch.Draw(Assets.Instance.InstructionsUi,
            new Rectangle(new Point(-16, -16), GameManager.Resolution),
            Assets.Instance.InstructionsUi.Bounds,
            Color.White);

        var instructions = StringMethods.BreakUpString(InstructionsString, 52);
        foreach (var (line, idx) in instructions.WithIndex())
        {
            var drawingData = Assets.Instance.Monogram24.GetDrawingData(line);
            GameManager.SpriteBatch.DrawTextDrawingData(drawingData,
                new Vector2(88, 120 + (drawingData.Dimensions.Y + 5) * idx),
                Color.Black);
        }
        
        GameManager.SpriteBatch.Draw(GameManager.Pixel,
            GameManager.WindowBounds,
            Color.Black * FadeLerper.GetLerpedValue());
    }

    public override void Update(GameTime gameTime)
    {
        if (State == InstructionsState.FadingIn)
        {
            FadeLerper.Update(gameTime);
            if (FadeLerper.IsDone())
            {
                SetState(InstructionsState.Normal);
            }
        }
        else if (State == InstructionsState.Normal)
        {
            if (InputManager.Instance.IsKeyPressed(InputAction.MenuSelect))
            {
                SetState(InstructionsState.FadingOut);
            }
        }
        else if (State == InstructionsState.FadingOut)
        {
            FadeLerper.Update(gameTime);
            if (FadeLerper.IsDone())
            {
                GameStateManager.Instance.PushState(new OverWorldGameState());
            }
        }
    }

    private void SetState(InstructionsState state)
    {
        State = state;
        switch (state)
        {
            case InstructionsState.FadingOut:
                FadeLerper = new LinearFloatLerper(0F, 1F, 1);
                break;
        }
    }

    private enum InstructionsState
    {
        FadingIn,
        Normal,
        FadingOut
    }
}