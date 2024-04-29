using CraftingRPG.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace CraftingRPG.Entities.EnemyInstances;

public abstract class BaseEnemyInstance : IEnemyInstance
{
    protected IEnemy EnemyInfo { get; set; }
    protected Vector2 Position { get; set; }
    protected Point Size { get; set; } = new(32, 32);
    protected int HitPoints { get; set; }
    protected double Depth { get; set; }
    protected Texture2D SpriteSheet { get; set; }

    public abstract RectangleF GetCollisionBox();
    public abstract Rectangle GetTextureRectangle();
    
    public virtual Vector2 GetPosition() => Position;
    public virtual double GetDepth() => Depth;
    public virtual Point GetSize() => Size;
    public virtual Texture2D GetSpriteSheet() => SpriteSheet;
    public virtual IEnemy GetEnemyInfo() => EnemyInfo;
    public virtual int GetCurrentHitPoints() => HitPoints;
    public virtual Vector2 SetPosition(Vector2 position) => Position = position;
    
    public virtual bool IncurDamage(int damage)
    {
        HitPoints -= damage;
        return HitPoints <= 0;
    }

    public virtual void Update(GameTime gameTime)
    {
    }
}