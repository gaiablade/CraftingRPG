using Microsoft.Xna.Framework;

namespace CraftingRPG.Lerpers;

public class Vector2Lerper : BaseLerper<Vector2>
{
    public Vector2Lerper(Vector2 start, Vector2 end, double duration) : base(start, end, duration)
    {
    }

    public override Vector2 GetLerpedValue()
    {
        return Vector2.Lerp(Start, End, (float)(Time / Duration));
    }
}