using CraftingRPG.Interfaces;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

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

    public RectangleF GetCollisionBox() =>
        new Rectangle((int)Position.X, (int)Position.Y, (int)GetSize().X, (int)GetSize().Y);

    public double GetDepth() => -1;

    public IDroppable GetDroppable() => Drop;

    public Vector2 GetPosition() => Position;

    public Vector2 GetSize() => new Vector2(32, 32);

    public int GetSpriteSheetIndex() => Drop.GetSpriteSheetIndex();

    public Vector2 SetPosition(Vector2 position) => Position = position;
}