using Microsoft.Xna.Framework;

namespace CraftingRPG.Interfaces;

public interface IMapObject
{
    public Point GetSize();
    public int GetSpriteSheetIndex();
    public Rectangle GetCollisionBox();
}
