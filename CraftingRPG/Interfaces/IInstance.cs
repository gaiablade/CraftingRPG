using CraftingRPG.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace CraftingRPG.Interfaces;

public interface IInstance
{
    public Vector2 GetPosition();
    public Vector2 SetPosition(Vector2 position);
    public double GetDepth();
    public Point GetSize();
    public RectangleF GetCollisionBox();
    public SpriteDrawingData GetDrawingData();
    public Texture2D GetSpriteSheet();
    public Rectangle GetTextureRectangle();
    public Vector2 GetMovementVector();
}
