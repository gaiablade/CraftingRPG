using System;
using CraftingRPG.EasingFunctions;
using CraftingRPG.Interfaces;
using Microsoft.Xna.Framework;

namespace CraftingRPG.Timers;

public class EaseOutTimer : ITimer
{
    private readonly double Duration;
    private double CurrentTime;
    private bool IsReverse;

    public EaseOutTimer(double duration, bool reverse = false)
    {
        Duration = duration;
        IsReverse = reverse;
        CurrentTime = 0;
    }

    public void Update(GameTime gameTime)
    {
        if (!IsReverse)
        {
            CurrentTime = Math.Min(Duration, CurrentTime + gameTime.ElapsedGameTime.TotalSeconds);
        }
        else
        {
            CurrentTime = Math.Max(0.0, CurrentTime - gameTime.ElapsedGameTime.TotalSeconds);
        }
    }

    public double GetPercent()
    {
        return Math.Min(1.0, Easing.EaseOutSine(CurrentTime / Duration));
    }

    public void Reset()
    {
        CurrentTime = 0.0;
    }

    public void SetReverse(bool reverse = true)
    {
        IsReverse = reverse;
    }

    public bool IsDone()
    {
        return !IsReverse ? CurrentTime >= Duration : CurrentTime <= 0;
    }
}