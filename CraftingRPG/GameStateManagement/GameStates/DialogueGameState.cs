using CraftingRPG.AssetManagement;
using CraftingRPG.Enums;
using CraftingRPG.Extensions;
using CraftingRPG.Global;
using CraftingRPG.InputManagement;
using CraftingRPG.Utility;
using Microsoft.Xna.Framework;

namespace CraftingRPG.GameStateManagement.GameStates;

public class DialogueGameState : BaseGameState
{
    private string[] MessageLines;
    
    public DialogueGameState(string message)
    {
        Flags.IsPaused = true;
        MessageLines = StringMethods.BreakUpString(message, 60);
    }
    
    public override void DrawUi()
    {
        GameManager.SpriteBatch.Draw(Assets.Instance.DialogueUi,
            new Rectangle(new Point(-16), GameManager.Resolution),
            Color.White);

        foreach (var (line, idx) in MessageLines.WithIndex())
        {
            var drawingData = Assets.Instance.Monogram24.GetDrawingData(line);
            GameManager.SpriteBatch.DrawTextDrawingData(drawingData,
                new Vector2(80, 400 + drawingData.Dimensions.Y * idx),
                Color.Black);
        }
    }

    public override void Update(GameTime gameTime)
    {
        if (InputManager.Instance.IsKeyPressed(InputAction.Interact))
        {
            InputManager.Instance.Debounce(InputAction.Interact);
            Flags.IsPaused = false;
            GameStateManager.Instance.PopState();
        }
    }
}