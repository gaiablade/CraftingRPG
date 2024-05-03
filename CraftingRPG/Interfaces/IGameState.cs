using Microsoft.Xna.Framework;

namespace CraftingRPG.Interfaces;

public interface IGameState
{
    public void Update(GameTime gameTime);
    public void DrawWorld();
    public void DrawUi();
}
