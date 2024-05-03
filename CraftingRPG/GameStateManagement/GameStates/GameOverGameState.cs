using CraftingRPG.AssetManagement;
using CraftingRPG.Extensions;
using Microsoft.Xna.Framework;

namespace CraftingRPG.GameStateManagement.GameStates;

public class GameOverGameState : BaseGameState
{
    public override void DrawUi()
    {
        var textData = Assets.Instance.Monogram24.GetDrawingData("GAME OVER");
        var screenCenter = GameManager.ScreenCenter;
        var position = Vector2.Subtract(screenCenter.ToVector2(), Vector2.Divide(textData.Dimensions, 2));
        GameManager.SpriteBatch.DrawTextDrawingData(textData,
            position,
            Color.Red);
        base.DrawUi();
    }
}