using Microsoft.Xna.Framework;

namespace CraftingRPG.Interfaces;

public interface ILerper<T>
{
    public bool IsDone();
    public T GetLerpedValue();
    public void Update(GameTime gameTime);
    public T GetStart();
    public T GetEnd();
    public void SetTime(double time);
}