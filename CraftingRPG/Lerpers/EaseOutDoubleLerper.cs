using CraftingRPG.EasingFunctions;
using CraftingRPG.Utility;

namespace CraftingRPG.Lerpers;

public class EaseOutDoubleLerper : BaseLerper<double>
{
    public EaseOutDoubleLerper(double start, double end, double duration) : base(start, end, duration)
    {
    }

    public override double GetLerpedValue()
    {
        return CustomMath.Lerp(Start, End, GetPercent());
    }

    protected override double GetPercent()
    {
        return Easing.EaseOutSine(base.GetPercent());
    }
}