using System;
using Microsoft.Xna.Framework;

namespace CraftingRPG.LerpPath;

public abstract class BaseLerpPath<T>
{
    protected T Start { get; set; }
    protected T End { get; set; }
    protected double Duration { get; set; } = 0;
    protected double Time { get; set; } = 0;

    public BaseLerpPath(T start, T end, double duration)
    {
        Start = start;
        End = end;
        Duration = duration;
    }

    public bool IsDone() => Time >= Duration;

    public abstract T GetLerpedValue();

    public virtual void Update(GameTime gameTime)
    {
        Time = Math.Min(Time + gameTime.ElapsedGameTime.TotalSeconds, Duration);
    }
}