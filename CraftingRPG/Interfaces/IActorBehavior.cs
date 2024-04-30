using Microsoft.Xna.Framework;

namespace CraftingRPG.Interfaces;

public interface IActorBehavior
{
    public void SetPosition(Vector2 position);
    public Vector2 GetPosition();
    public void Update(GameTime gameTime);
}