using Microsoft.Xna.Framework;

namespace CraftingRPG.Interfaces;

public interface IState
{
    public void Update(GameTime gameTime);
    public void DrawWorld();
    public void DrawUI();
}
