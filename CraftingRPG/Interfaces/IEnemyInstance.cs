using CraftingRPG.Lerpers;
using Microsoft.Xna.Framework;

namespace CraftingRPG.Interfaces;

public interface IEnemyInstance : IInstance
{
    public IEnemy GetEnemyInfo();
    public int GetCurrentHitPoints();
    public void IncurDamage(int damage);
    public void Update(GameTime gameTime);
    public bool IsAttacking();
    public Rectangle GetAttackHitBox();
    public void SetKnockBack(Vector2Lerper knockBackPath);
    public Vector2 GetAttackAngle();
    public bool IsDefeated();
    public void OnDeath();
}
