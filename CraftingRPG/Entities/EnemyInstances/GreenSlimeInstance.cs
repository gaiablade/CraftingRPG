using CraftingRPG.AssetManagement;
using CraftingRPG.Enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace CraftingRPG.Entities.EnemyInstances;

public class GreenSlimeInstance : BaseEnemyInstance
{
    public GreenSlimeInstance()
    {
        EnemyInfo = new GreenSlime();
        HitPoints = EnemyInfo.GetMaxHitPoints();
        SpriteSheet = Assets.Instance.SlimeSpriteSheet;
    }

    public override RectangleF GetCollisionBox() => new(Position.X + 8, Position.Y + 11, 16, 13);

    public override Texture2D GetSpriteSheet()
    {
        return Assets.Instance.SlimeSpriteSheet;
    }

    public override Rectangle GetTextureRectangle()
    {
        return new Rectangle(0, 0, 32, 32);
    }
}