using Microsoft.Xna.Framework;

namespace CraftingRPG.Interfaces;

public interface ITimer
{
    public void Update(GameTime gameTime);
    public double GetPercent();
    public void Reset();
    public void SetReverse(bool reverse = true);
    public bool IsDone();
}