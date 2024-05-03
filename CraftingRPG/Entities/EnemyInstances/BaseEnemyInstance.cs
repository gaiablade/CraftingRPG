using CraftingRPG.AssetManagement;
using CraftingRPG.Graphics;
using CraftingRPG.Interfaces;
using CraftingRPG.Lerpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace CraftingRPG.Entities.EnemyInstances;

public abstract class BaseEnemyInstance : IEnemyInstance
{
    protected Vector2 MovementVector;
    
    protected IEnemy EnemyInfo { get; set; }
    protected Vector2 Position { get; set; }
    protected Point Size { get; set; } = new(32, 32);
    protected int HitPoints { get; set; }
    protected double Depth { get; set; }
    protected Texture2D SpriteSheet { get; set; }
    protected Vector2Lerper KnockBackPath { get; set; }

    public abstract RectangleF GetCollisionBox();
    public abstract SpriteDrawingData GetDrawingData();
    public abstract Rectangle GetTextureRectangle();
    public virtual Vector2 GetMovementVector() => MovementVector;
    public virtual Vector2 GetPosition() => Position;
    public Vector2 Move(Vector2 movementVector) => SetPosition(Vector2.Add(GetPosition(), movementVector));
    public virtual double GetDepth() => Depth;
    public virtual Point GetSize() => Size;
    public virtual Texture2D GetSpriteSheet() => SpriteSheet;
    public virtual IEnemy GetEnemyInfo() => EnemyInfo;
    public virtual int GetCurrentHitPoints() => HitPoints;

    public virtual Vector2 SetPosition(Vector2 position)
    {
        Position = position;
        return Position;
    }

    public virtual void IncurDamage(int damage) => HitPoints -= damage;

    public virtual void Update(GameTime gameTime)
    {
        if (KnockBackPath != null)
        {
            KnockBackPath.Update(gameTime);
        }
    }

    public abstract bool IsAttacking();
    public abstract Rectangle GetAttackHitBox();

    public virtual void SetKnockBack(Vector2Lerper knockBackPath) => KnockBackPath = knockBackPath;
    public virtual Vector2 GetAttackAngle()
    {
        return Vector2.One;
    }

    public virtual bool IsDefeated() => GetCurrentHitPoints() <= 0;

    public virtual void OnDeath()
    {
        Assets.Instance.EnemyDeath02.Play(0.3F, 0F, 0F);
    }
}