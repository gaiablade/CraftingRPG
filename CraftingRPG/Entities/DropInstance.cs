using CraftingRPG.Constants;
using CraftingRPG.Interfaces;
using Microsoft.Xna.Framework;

namespace CraftingRPG.Entities;

public class DropInstance : IDropInstance
{
    private Vector2 Position = Vector2.Zero;
    private IDroppable Drop;

    public DropInstance(IDroppable inst)
    {
        Drop = inst;
    }

    public DropInstance(IDroppable inst, Vector2 pos)
    {
        Position = pos;
        Drop = inst;
    }

    public Rectangle GetCollisionBox() => new Rectangle(0, 0, 0, 0);

    public float GetDepth() => -1;

    public IDroppable GetDroppable() => Drop;

    public Vector2 GetPosition() => Position;

    public Vector2 GetSize() => new Vector2(32, 32);

    public int GetSpriteSheetIndex() => Drop.GetSpriteSheetIndex();
}
