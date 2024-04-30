using CraftingRPG.Interfaces;
using Microsoft.Xna.Framework;

namespace CraftingRPG.GameStateManagement;

public abstract class BaseState : IState
{
    public virtual void Update(GameTime gameTime)
    {
    }

    public virtual void DrawWorld()
    {
    }

    public virtual void DrawUI()
    {
    }
}