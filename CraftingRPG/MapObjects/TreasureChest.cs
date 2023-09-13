using CraftingRPG.Constants;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using Microsoft.Xna.Framework;

namespace CraftingRPG.MapObjects;

public class TreasureChest : IMapObject
{
    public ChestId Id;

    public Rectangle GetCollisionBox() => new Rectangle(0, 16, 32, 16);

    public Point GetSize() => new Point(32, 32);

    public int GetSpriteSheetIndex() => SpriteIndex.TreasureChest;
}
