using System;
using CraftingRPG.Interfaces;
using Microsoft.Xna.Framework;

namespace CraftingRPG.Lerpers;

public abstract class BaseLerper<T> : ILerper<T>
{
    protected T Start { get; set; }
    protected T End { get; set; }
    protected double Duration { get; set; }
    protected double Time { get; set; }

    protected BaseLerper(T start, T end, double duration)
    {
        Start = start;
        End = end;
        Duration = duration;
    }

    public virtual bool IsDone() => Time >= Duration;

    public abstract T GetLerpedValue();

    public virtual void Update(GameTime gameTime)
    {
        Time = Math.Min(Time + gameTime.ElapsedGameTime.TotalSeconds, Duration);
    }

    public virtual T GetStart() => Start;
    public virtual T GetEnd() => End;

    public void SetTime(double time) => Time = time;

    protected virtual double GetPercent() => Time / Duration;
}