using Microsoft.Xna.Framework;

namespace CraftingRPG.SpriteAnimation.CustomAnimations;

public class PlayerDeathAnimation : Animation
{
    private readonly double[] FrameDurations = new[]
    {
        1.0,
        1.0,
        2.0
    };
    
    public PlayerDeathAnimation() : base(3, 0, new Point(48, 48), false, 0, 432)
    {
    }

    protected override bool IsCurrentFrameOver()
    {
        return FrameTimer >= FrameDurations[CurrentAnimationFrame];
    }
}