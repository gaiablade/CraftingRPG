using CraftingRPG.Utility;

namespace CraftingRPG.Lerpers;

public class FloatLerper : BaseLerper<float>
{
    public FloatLerper(float start, float end, double duration) : base(start, end, duration)
    {
    }

    public override float GetLerpedValue()
    {
        return (float)CustomMath.Lerp(Start, End, GetPercent());
    }
}