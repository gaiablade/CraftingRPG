using CraftingRPG.Interfaces;
using Microsoft.Xna.Framework;

namespace CraftingRPG.ActorBehaviors;

public class BaseBehavior : IActorBehavior
{
    public virtual void SetPosition(Vector2 position)
    {
    }

    public virtual Vector2 GetPosition()
    {
        return Vector2.Zero;
    }

    public virtual void Update(GameTime gameTime)
    {
    }
}