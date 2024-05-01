using CraftingRPG.LerpPath;
using Microsoft.Xna.Framework;

namespace CraftingRPG.Interfaces;

public interface IEnemyInstance : IInstance
{
    public IEnemy GetEnemyInfo();
    public int GetCurrentHitPoints();
    public bool IncurDamage(int damage);
    public void Update(GameTime gameTime);
    public bool IsAttacking();
    public Rectangle GetAttackHitBox();
    public void SetKnockBack(Vector2LerpPath knockBackPath);
}
