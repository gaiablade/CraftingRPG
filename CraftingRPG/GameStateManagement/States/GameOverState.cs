using CraftingRPG.AssetManagement;
using CraftingRPG.Extensions;
using Microsoft.Xna.Framework;

namespace CraftingRPG.GameStateManagement.States;

public class GameOverState : BaseState
{
    public override void DrawUI()
    {
        var textData = Assets.Instance.Monogram24.GetDrawingData("GAME OVER");
        var screenCenter = GameManager.ScreenCenter;
        var position = Vector2.Subtract(screenCenter.ToVector2(), Vector2.Divide(textData.Dimensions, 2));
        GameManager.SpriteBatch.DrawTextDrawingData(textData,
            position,
            Color.Red);
        base.DrawUI();
    }
}