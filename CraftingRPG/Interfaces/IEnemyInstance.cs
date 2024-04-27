using Microsoft.Xna.Framework;

namespace CraftingRPG.Interfaces;

public interface IEnemyInstance : IInstance
{
    public IEnemy GetEnemy();
    public int GetCurrentHitPoints();
    public bool IncurDamage(int damage);
    public void UpdateAnimation(GameTime gameTime);
}
