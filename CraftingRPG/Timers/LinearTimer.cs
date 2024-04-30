using System;

namespace CraftingRPG.Timers;

public class LinearTimer : BaseTimer
{
    public LinearTimer(double duration, bool reverse = false)
    {
        Duration = duration;
        IsReverse = reverse;
        CurrentTime = 0;
    }

    public override double GetPercent()
    {
        return Math.Min(1.0, CurrentTime / Duration);
    }
}