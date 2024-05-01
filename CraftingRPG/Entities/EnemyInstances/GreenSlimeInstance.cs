using CraftingRPG.ActorBehaviors.Behaviors;
using CraftingRPG.AssetManagement;
using CraftingRPG.Constants;
using CraftingRPG.Enemies;
using CraftingRPG.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace CraftingRPG.Entities.EnemyInstances;

public class GreenSlimeInstance : BaseEnemyInstance
{
    private readonly SlimeBehavior Behavior;
    private int FacingDirection = Direction.Right;

    public GreenSlimeInstance()
    {
        EnemyInfo = new GreenSlime();
        HitPoints = EnemyInfo.GetMaxHitPoints();
        SpriteSheet = Assets.Instance.SlimeSpriteSheet;
        Behavior = new();
        Behavior.SetPosition(Position);
    }

    public override RectangleF GetCollisionBox() => new(Position.X + 8, Position.Y + 11, 16, 13);

    public override SpriteDrawingData GetDrawingData() => new()
    {
        Texture = GetSpriteSheet(),
        SourceRectangle = GetTextureRectangle(),
        Flip = FacingDirection == Direction.Left
    };

    public override Texture2D GetSpriteSheet() => Assets.Instance.SlimeSpriteSheet;

    public override Vector2 SetPosition(Vector2 position)
    {
        Position = position;
        Behavior.SetPosition(Position);
        return Position;
    }

    public override Rectangle GetTextureRectangle()
    {
        return Behavior.CurrentAnimation.GetSourceRectangle();
    }

    public override void Update(GameTime gameTime)
    {
        Behavior.Update(gameTime);
        MovementVector = Behavior.GetMovementVector();

        FacingDirection = Behavior.GetMovementVector().X switch
        {
            < 0 => Direction.Left,
            > 0 => Direction.Right,
            _ => FacingDirection
        };
        
        base.Update(gameTime);
    }

    public override bool IsAttacking() => Behavior.GetBehaviorState() == SlimeActorBehaviorState.Attacking;

    public override Rectangle GetAttackHitBox() => Behavior.GetAttackHitBox();
}