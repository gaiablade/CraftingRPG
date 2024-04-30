using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CraftingRPG.Graphics;

public class SpriteDrawingData
{
    public Texture2D Texture { get; set; }
    public Rectangle SourceRectangle { get; set; }
    public Vector2 Origin { get; set; } = Vector2.Zero;
    public float Rotation { get; set; } = 0F;
    public bool Flip { get; set; } = false;
    public float Scale { get; set; } = 1F;
}