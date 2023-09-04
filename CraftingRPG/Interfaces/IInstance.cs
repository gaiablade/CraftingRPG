using Microsoft.Xna.Framework;

namespace CraftingRPG.Interfaces;

public interface IInstance
{
    public Vector2 GetPosition();
    public Vector2 SetPosition(Vector2 position);
    public float GetDepth();
    public Vector2 GetSize();
    public int GetSpriteSheetIndex();
    public Rectangle GetCollisionBox();
}
