using Microsoft.Xna.Framework;

namespace CraftingRPG.SpriteAnimation.CustomAnimations;

public class PlayerDeathAnimation : BaseCustomAnimation
{
    public PlayerDeathAnimation() : base(3, 0, new Point(48, 48), false, 0, 432)
    {
        SetFrameDurations(new[]
        {
            1.0,
            1.0,
            2.0
        });
    }
}