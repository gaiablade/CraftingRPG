using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CraftingRPG.Graphics;

public class TextDrawingData
{
    public SpriteFont Font { get; set; }
    public string Message { get; set; }
    public Vector2 Dimensions { get; set; }
    public float Rotation { get; set; } = 0F;
    public Vector2 Origin { get; set; } = Vector2.Zero;
    public float Scale { get; set; } = 1F;
}