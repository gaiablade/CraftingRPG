using CraftingRPG.AssetManagement;
using CraftingRPG.Enums;
using CraftingRPG.InputManagement;
using Microsoft.Xna.Framework;

namespace CraftingRPG.GameStateManagement.GameStates;

public class MainMenuGameState : BaseGameState
{
    public override void DrawWorld()
    {
    }

    public override void DrawUI()
    {
        GameManager.SpriteBatch.Draw(Assets.Instance.TitleUi,
            GameManager.WindowBounds,
            Color.White);
    }

    public override void Update(GameTime gameTime)
    {
        if (InputManager.Instance.GetKeyPressState(InputAction.MenuSelect) == KeyPressState.Pressed)
        {
            GameStateManager.Instance.PushState<OverWorldGameState>();
        }
    }
}
