using Microsoft.Xna.Framework;

namespace CraftingRPG.Interfaces;

public interface IAnimation
{
    public void Update(GameTime gameTime);
    public void Reset();
    public Rectangle GetSourceRectangle();
    public bool IsAnimationOver();
    public int GetCurrentFrame();
    public double GetDuration();
}