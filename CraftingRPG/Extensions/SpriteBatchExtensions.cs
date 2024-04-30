using CraftingRPG.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CraftingRPG.Extensions;

public static class SpriteBatchExtensions
{
    public static void DrawTextDrawingData(this SpriteBatch spriteBatch, TextDrawingData drawingData, Vector2 position,
        Color color)
    {
        spriteBatch.DrawString(drawingData.Font, drawingData.Message, position, color, drawingData.Rotation,
            drawingData.Origin, drawingData.Scale, SpriteEffects.None, 0F);
    }
}