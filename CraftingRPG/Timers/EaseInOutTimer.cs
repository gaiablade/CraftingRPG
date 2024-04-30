using System;
using CraftingRPG.EasingFunctions;

namespace CraftingRPG.Timers;

public class EaseInOutTimer : BaseTimer
{
    public EaseInOutTimer(double duration, bool reverse = false)
    {
        Duration = duration;
        IsReverse = reverse;
        CurrentTime = 0;
    }
    
    public override double GetPercent()
    {
        return Math.Min(1.0, Easing.EaseInOutSine(CurrentTime / Duration));
    }
}