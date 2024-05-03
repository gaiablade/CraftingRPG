using CraftingRPG.Utility;

namespace CraftingRPG.Lerpers;

public class LinearFloatLerper : BaseLerper<float>
{
    public LinearFloatLerper(float start, float end, double duration) : base(start, end, duration)
    {
    }

    public override float GetLerpedValue()
    {
        return (float)CustomMath.Lerp(Start, End, GetPercent());
    }
}