using CraftingRPG.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace CraftingRPG.Extensions;

public static class SpriteFontExtensions
{
    public static TextDrawingData GetDrawingData(this SpriteFont spriteFont, string message)
    {
        var dimensions = spriteFont.MeasureString(message);
        return new TextDrawingData
        {
            Font = spriteFont,
            Dimensions = dimensions,
            Message = message
        };
    }
}