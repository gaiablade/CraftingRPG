using CraftingRPG.AssetManagement;
using CraftingRPG.Enums;
using CraftingRPG.Extensions;
using CraftingRPG.InputManagement;
using CraftingRPG.Interfaces;
using CraftingRPG.Lerpers;
using CraftingRPG.SourceRectangleProviders;
using Microsoft.Xna.Framework;

namespace CraftingRPG.GameStateManagement.GameStates;

public class KeybindingsGameState : BaseGameState
{
    private readonly ISourceRectangleProvider<InputAction> InputActionSourceRectangleProvider;
    private ILerper<Vector2> MenuPositionLerper;
    private ILerper<float> BackgroundFadeLerper;
    private KeybindingState State = KeybindingState.SlidingIn;

    public KeybindingsGameState()
    {
        InputActionSourceRectangleProvider = new InputActionKeySourceRectangleProvider();
        MenuPositionLerper =
            new EaseOutVector2Lerper(new Vector2(-16, -16 - GameManager.Resolution.Y), new Vector2(-16, -16), 0.5);
        BackgroundFadeLerper = new LinearFloatLerper(0F, 0.75F, 0.5);
    }

    public override void DrawUi()
    {
        var position = MenuPositionLerper.GetLerpedValue();
        
        GameManager.SpriteBatch.Draw(GameManager.Pixel,
            GameManager.WindowBounds,
            Color.Black * BackgroundFadeLerper.GetLerpedValue());

        GameManager.SpriteBatch.Draw(Assets.Instance.QuestUi,
            new Rectangle(position.ToPoint(), GameManager.Resolution),
            Assets.Instance.QuestUi.Bounds,
            Color.White);

        var actions = new[]
        {
            new
            {
                Action = InputAction.MoveNorth,
                Description = "Move North"
            },
            new
            {
                Action = InputAction.MoveSouth,
                Description = "Move South"
            },
            new
            {
                Action = InputAction.MoveEast,
                Description = "Move East"
            },
            new
            {
                Action = InputAction.MoveWest,
                Description = "Move West"
            },
            new
            {
                Action = InputAction.Attack,
                Description = "Attack"
            },
            new
            {
                Action = InputAction.ExitMenu,
                Description = "Close Menu"
            },
            new
            {
                Action = InputAction.OpenCraftingMenu,
                Description = "Open Crafting Menu"
            },
            new
            {
                Action = InputAction.OpenInventoryMenu,
                Description = "Open Inventory"
            },
            new
            {
                Action = InputAction.OpenQuestsMenu,
                Description = "View Quests"
            },
            new
            {
                Action = InputAction.MenuSelect,
                Description = "Select (in menus)"
            },
            new
            {
                Action = InputAction.OpenKeybindings,
                Description = "View Keybindings"
            },
        };

        foreach (var (action, idx) in actions.WithIndex())
        {
            var sourceRectangle = InputActionSourceRectangleProvider.GetSourceRectangle(action.Action);
            var drawingData = Assets.Instance.Monogram24.GetDrawingData(action.Description);
            var x = position.X + 116;
            var y = position.Y + 136 + (sourceRectangle.Height + 5) * idx;
            GameManager.SpriteBatch.Draw(Assets.Instance.KeyIconSpriteSheet,
                new Vector2(x, y),
                sourceRectangle,
                Color.White);
            GameManager.SpriteBatch.DrawTextDrawingData(drawingData,
                new Vector2(x + sourceRectangle.Width + 20, y),
                Color.White);
        }
    }

    public override void Update(GameTime gameTime)
    {
        MenuPositionLerper.Update(gameTime);
        BackgroundFadeLerper.Update(gameTime);

        var lerpersDone = MenuPositionLerper.IsDone() && BackgroundFadeLerper.IsDone();

        if (lerpersDone && State == KeybindingState.SlidingIn)
        {
            SetState(KeybindingState.Normal);
        }
        else if (lerpersDone && State == KeybindingState.SlidingOut)
        {
            GameStateManager.Instance.PopState();
        }

        if (State == KeybindingState.Normal && InputManager.Instance.IsKeyPressed(InputAction.ExitMenu))
        {
            SetState(KeybindingState.SlidingOut);
        }
    }

    private void SetState(KeybindingState state)
    {
        State = state;
        switch (state)
        {
            case KeybindingState.SlidingOut:
                BackgroundFadeLerper = new LinearFloatLerper(0.75F, 0F, 0.5);
                MenuPositionLerper = new EaseOutVector2Lerper(new Vector2(-16, -16),
                    new Vector2(-16, -16 - GameManager.Resolution.Y), 0.5);
                break;
        }
    }

    private enum KeybindingState
    {
        SlidingIn,
        Normal,
        SlidingOut
    }
}