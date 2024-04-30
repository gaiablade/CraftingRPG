using CraftingRPG.ActorBehaviors.Behaviors;
using CraftingRPG.AssetManagement;
using CraftingRPG.Constants;
using CraftingRPG.Enemies;
using CraftingRPG.Graphics;
using CraftingRPG.SpriteAnimation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace CraftingRPG.Entities.EnemyInstances;

public class GreenSlimeInstance : BaseEnemyInstance
{
    private readonly SlimeBehavior Behavior;
    private readonly Animation IdleAnimation;
    private readonly Animation MovingAnimation;
    private int FacingDirection = Direction.Right;
    
    public GreenSlimeInstance()
    {
        EnemyInfo = new GreenSlime();
        HitPoints = EnemyInfo.GetMaxHitPoints();
        SpriteSheet = Assets.Instance.SlimeSpriteSheet;
        Behavior = new();
        Behavior.SetPosition(Position);

        IdleAnimation = new Animation(4, 0.2, new Point(32, 32));
        MovingAnimation = new Animation(6, 0.1, new Point(32, 32), true, 0, 32);
    }

    public override RectangleF GetCollisionBox() => new(Position.X + 8, Position.Y + 11, 16, 13);
    public override SpriteDrawingData GetDrawingData()
    {
        return new SpriteDrawingData
        {
            Texture = GetSpriteSheet(),
            SourceRectangle = GetTextureRectangle(),
            Flip = FacingDirection == Direction.Left
        };
    }

    public override Texture2D GetSpriteSheet()
    {
        return Assets.Instance.SlimeSpriteSheet;
    }

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
        if (Behavior.GetBehaviorState() == SlimeActorBehaviorState.Idle)
        {
            IdleAnimation.Update(gameTime);
        }
        else
        {
            MovingAnimation.Update(gameTime);
        }
        
        Behavior.Update(gameTime);
        MovementVector = Behavior.GetMovementVector();
        
        FacingDirection = Behavior.GetMovementVector().X switch
        {
            < 0 => Direction.Left,
            > 0 => Direction.Right,
            _ => FacingDirection
        };
    }

    public override bool IsAttacking()
    {
        return Behavior.GetBehaviorState() == SlimeActorBehaviorState.Attacking;
    }

    public override Rectangle GetAttackHitBox()
    {
        return Behavior.GetAttackHitBox();
    }
}