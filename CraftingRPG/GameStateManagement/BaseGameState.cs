using CraftingRPG.Interfaces;
using Microsoft.Xna.Framework;

namespace CraftingRPG.GameStateManagement;

public abstract class BaseGameState : IGameState
{
    public virtual void Update(GameTime gameTime)
    {
    }

    public virtual void DrawWorld()
    {
    }

    public virtual void DrawUi()
    {
    }
}