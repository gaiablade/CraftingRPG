using CraftingRPG.AssetManagement;
using CraftingRPG.Enemies;
using CraftingRPG.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace CraftingRPG.Entities.EnemyInstances;

public class GreenSlimeInstance : IEnemyInstance
{
    private GreenSlime enemyInfo;
    private Vector2 Position;
    private Vector2 Size = new Vector2(32, 32);
    private int HitPoints;

    public GreenSlimeInstance()
    {
        enemyInfo = new GreenSlime();
        HitPoints = enemyInfo.GetMaxHitPoints();
    }
    
    public Vector2 GetPosition() => Position;

    public Vector2 SetPosition(Vector2 position) => Position = position;

    public double GetDepth() => Position.Y + 32;

    public Vector2 GetSize() => Size;

    public RectangleF GetCollisionBox() => new(Position.X + 8, Position.Y + 11, 16, 13);

    public Texture2D GetSpriteSheet()
    {
        return Assets.Instance.SlimeSpriteSheet;
    }

    public Rectangle GetTextureRectangle()
    {
        return new Rectangle(0, 0, 32, 32);
    }

    public IEnemy GetEnemy()
    {
        return new GreenSlime();
    }

    public int GetCurrentHitPoints() => HitPoints;

    public bool IncurDamage(int damage)
    {
        HitPoints -= damage;
        return HitPoints <= 0;
    }

    public void UpdateAnimation(GameTime gameTime)
    {
        throw new System.NotImplementedException();
    }
}