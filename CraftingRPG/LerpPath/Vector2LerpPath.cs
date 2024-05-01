using Microsoft.Xna.Framework;

namespace CraftingRPG.LerpPath;

public class Vector2LerpPath : BaseLerpPath<Vector2>
{
    public Vector2LerpPath(Vector2 start, Vector2 end, double duration) : base(start, end, duration)
    {
    }

    public override Vector2 GetLerpedValue()
    {
        return Vector2.Lerp(Start, End, (float)(Time / Duration));
    }
}