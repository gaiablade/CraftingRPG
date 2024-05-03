using Microsoft.Xna.Framework;

namespace CraftingRPG.SpriteAnimation.CustomAnimations;

public abstract class BaseCustomAnimation : Animation
{
    private double[] FrameDurations;

    protected BaseCustomAnimation(int frames, double interval, Point size, bool loop = true, int x = 0, int y = 0) : base(
        frames, interval, size, loop, x, y)
    {
    }
    
    protected override bool IsCurrentFrameOver()
    {
        return FrameTimer >= FrameDurations[CurrentAnimationFrame];
    }

    protected void SetFrameDurations(double[] durations)
    {
        FrameDurations = durations;
    }
}