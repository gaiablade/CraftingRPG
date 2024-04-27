using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace CraftingRPG.Interfaces;

public interface IInstance
{
    public Vector2 GetPosition();
    public Vector2 SetPosition(Vector2 position);
    public double GetDepth();
    public Vector2 GetSize();
    public RectangleF GetCollisionBox();
    public Texture2D GetSpriteSheet();
    public Rectangle GetTextureRectangle();
}
