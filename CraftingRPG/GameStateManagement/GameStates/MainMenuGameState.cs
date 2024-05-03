using CraftingRPG.AssetManagement;
using CraftingRPG.Enums;
using CraftingRPG.Extensions;
using CraftingRPG.InputManagement;
using CraftingRPG.Interfaces;
using CraftingRPG.SourceRectangleProviders;
using Microsoft.Xna.Framework;

namespace CraftingRPG.GameStateManagement.GameStates;

public class MainMenuGameState : BaseGameState
{
    private ISourceRectangleProvider<InputAction> SourceRectangleProvider;

    public MainMenuGameState()
    {
        SourceRectangleProvider = new InputActionKeySourceRectangleProvider();
    }
    
    public override void DrawWorld()
    {
    }

    public override void DrawUI()
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
    }

    public override void Update(GameTime gameTime)
    {
        if (InputManager.Instance.GetKeyPressState(InputAction.MenuSelect) == KeyPressState.Pressed)
        {
            GameStateManager.Instance.PushState<OverWorldGameState>();
        }
    }
}
