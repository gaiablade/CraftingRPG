using System;
using CraftingRPG.Interfaces;
using Microsoft.Xna.Framework;

namespace CraftingRPG.Timers;

public abstract class BaseTimer : ITimer
{
    protected double Duration;
    protected double CurrentTime;
    protected bool IsReverse;
    
    public abstract double GetPercent();
    
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
    public void Reset() => Set(0.0);
    public void Set(double time) => CurrentTime = time;
    public void MaxOut() => CurrentTime = Duration;
    public void SetReverse(bool reverse = true)
    {
        IsReverse = reverse;
    }
    public bool GetReverse() => IsReverse;

    public bool IsDone()
    {
        if (IsReverse)
        {
            return CurrentTime <= 0.0;
        }
        else
        {
            return CurrentTime >= Duration;
        }
    }
}