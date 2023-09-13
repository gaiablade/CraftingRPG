using CraftingRPG.Constants;
using CraftingRPG.Interfaces;
using Microsoft.Xna.Framework;

namespace CraftingRPG.MapObjects;

public class Crate : IMapObject
{
    public Point GetSize() => new Point(32, 32);
    public int GetSpriteSheetIndex() => SpriteIndex.Crate;
    public Rectangle GetCollisionBox() => new Rectangle(0, 16, 32, 16);
    public void OnInteract()
    {
    }
}
