using System;
using CraftingRPG.EasingFunctions;

namespace CraftingRPG.Timers;

public class EaseOutTimer : BaseTimer
{
    public EaseOutTimer(double duration, bool reverse = false)
    {
        Duration = duration;
        IsReverse = reverse;
        CurrentTime = 0;
    }

    public override double GetPercent()
    {
        return Math.Min(1.0, Easing.EaseOutSine(CurrentTime / Duration));
    }
}