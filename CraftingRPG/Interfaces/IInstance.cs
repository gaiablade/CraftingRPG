using Microsoft.Xna.Framework;

namespace CraftingRPG.Interfaces;

public interface IInstance
{
    public Point GetPosition();
    public int GetDepth();
    public Vector2 GetSize();
    public int GetSpriteSheetIndex();
}
