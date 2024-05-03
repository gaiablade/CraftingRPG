using CraftingRPG.EasingFunctions;
using Microsoft.Xna.Framework;

namespace CraftingRPG.Lerpers;

public class EaseOutVector2Lerper : BaseLerper<Vector2>
{
    public EaseOutVector2Lerper(Vector2 start, Vector2 end, double duration) : base(start, end, duration)
    {
    }

    public override Vector2 GetLerpedValue()
    {
        return Vector2.Lerp(Start, End, (float)GetPercent());
    }

    protected override double GetPercent()
    {
        return Easing.EaseOutSine(base.GetPercent());
    }
}