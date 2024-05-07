using System.Collections.Generic;
using CraftingRPG.AssetManagement;
using CraftingRPG.Constants;
using CraftingRPG.Enums;
using CraftingRPG.Graphics;
using CraftingRPG.Interfaces;
using CraftingRPG.Items;
using CraftingRPG.Lerpers;
using CraftingRPG.RecipeManagement.Recipes;
using CraftingRPG.SpriteAnimation;
using CraftingRPG.SpriteAnimation.CustomAnimations;
using CraftingRPG.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace CraftingRPG.Player;

public class PlayerInstance : IInstance
{
    // Constants
    private const float MovementSpeed = 100F;
    private const int AttackFrameLength = 5;
    private static readonly Point SpriteSize = new(48, 48);
    private static readonly Point Size = new(48, 48);

    // Player Info (Stats, Inventory, Quests, etc.)
    private PlayerInfo Info;

    // Movement related
    private Vector2 Position;
    private Vector2 MovementVector;

    //private Vector2 Center => Vector2.Add(Position, new Vector2(24, 36));
    private int FacingDirection = Direction.Down;
    private int PreviousFacingDirection;
    private int CurrentFacingDirection = Direction.Down;
    private bool IsAttacking;
    private bool PreviousIsAttacking;
    private bool CurrentIsAttacking;
    private bool IsWalking;
    private bool PreviousIsWalking;
    private bool CurrentIsWalking;
    private ILerper<Vector2> KnockBackLerper = new LinearVector2Lerper(Vector2.Zero, Vector2.Zero, 0);

    // HP and IFrames
    private ITimer InvulnerabilityTimer;
    private int HitPoints;

    // Map drops
    private IList<IDropInstance> DropsBelowPlayer { get; set; } = new List<IDropInstance>();
    private bool IsAboveDrop = false;

    // Attack hitbox
    private RectangleF HitBox;

    private Animation IdleSouthAnimation;
    private Animation IdleNorthAnimation;
    private Animation IdleSideAnimation;
    private Animation WalkingSouthAnimation;
    private Animation WalkingNorthAnimation;
    private Animation WalkingSideAnimation;
    private Animation AttackingSouthAnimation;
    private Animation AttackingNorthAnimation;
    private Animation AttackingSideAnimation;
    private PlayerDeathAnimation DeathAnimation;

    private Animation CurrentAnimation;

    public PlayerInstance(PlayerInfo info)
    {
        const double idleInterval = 0.1;
        const double walkInterval = 0.1;
        const double attackInterval = 0.1;

        Info = info;
        IdleSouthAnimation = new Animation(6, idleInterval, SpriteSize);
        IdleSideAnimation = new Animation(6, idleInterval, SpriteSize, true, 0, 48);
        IdleNorthAnimation = new Animation(6, idleInterval, SpriteSize, true, 0, 96);
        WalkingSouthAnimation = new Animation(6, walkInterval, SpriteSize, true, 0, 144);
        WalkingSideAnimation = new Animation(6, walkInterval, SpriteSize, true, 0, 192);
        WalkingNorthAnimation = new Animation(6, walkInterval, SpriteSize, true, 0, 240);
        AttackingSouthAnimation = new Animation(4, attackInterval, SpriteSize, false, 0, 288);
        AttackingSideAnimation = new Animation(4, attackInterval, SpriteSize, false, 0, 336);
        AttackingNorthAnimation = new Animation(4, attackInterval, SpriteSize, false, 0, 384);
        DeathAnimation = new PlayerDeathAnimation();
        CurrentAnimation = IdleSouthAnimation;
        InvulnerabilityTimer = new LinearTimer(1.0);
        InvulnerabilityTimer.Set(1.0);
        HitPoints = Info.Stats.MaxHitPoints;
    }

    // IInstance methods
    public Vector2 GetPosition() => Position;
    public Vector2 SetPosition(Vector2 position) => Position = position;
    public Vector2 Move(Vector2 movementVector) => SetPosition(Vector2.Add(GetPosition(), movementVector));
    public double GetDepth() => Position.Y + GetSize().Y;
    public Point GetSize() => Size;
    public Rectangle GetBounds() => new Rectangle((int)Position.X, (int)Position.Y, GetSize().X, GetSize().Y);
    public RectangleF GetCollisionBox() => new(new Point2(Position.X + 19, Position.Y + 32), new Size2(11, 9));

    public SpriteDrawingData GetDrawingData() => new()
    {
        Texture = GetSpriteSheet(),
        SourceRectangle = GetSourceRectangle(),
        Flip = FacingDirection == Direction.Left
    };

    public Texture2D GetSpriteSheet() => Assets.Instance.PlayerSpriteSheet;
    public Rectangle GetTextureRectangle() => CurrentAnimation.GetSourceRectangle();

    public Vector2 GetMovementVector()
    {
        if (!KnockBackLerper.IsDone())
        {
            var lerpPosition = KnockBackLerper.GetLerpedValue();
            var knockBackMovementVector = Vector2.Subtract(lerpPosition, Position);
            return knockBackMovementVector;
        }

        return MovementVector;
    }

    // Custom methods
    public PlayerInfo GetInfo() => Info;
    public Vector2 GetCenter() => GetPosition() + new Vector2(24, 36);
    public int GetHitPoints() => HitPoints;
    public bool GetIsAttacking() => IsAttacking;
    public bool GetIsWalking() => IsWalking;
    public void SetIsAttacking(bool isAttacking = true) => IsAttacking = isAttacking;
    public void SetIsWalking(bool isWalking = true) => IsWalking = isWalking;
    public float GetMovementSpeed() => MovementSpeed;
    public bool GetIsAboveDrop() => IsAboveDrop;
    public void SetIsAboveDrop(bool isAboveDrop) => IsAboveDrop = isAboveDrop;
    public IList<IDropInstance> GetDropsBelowPlayer() => DropsBelowPlayer;
    public void SetDropsBelowPlayer(IList<IDropInstance> drops) => DropsBelowPlayer = drops;
    public bool GetIsInvulnerable() => !InvulnerabilityTimer.IsDone();
    public void SetInvulnerable() => InvulnerabilityTimer.Reset();
    public void IncurDamage(int damage) => HitPoints -= damage;
    public void SetKnockBack(ILerper<Vector2> lerper) => KnockBackLerper = lerper;
    public int GetFacingDirection() => FacingDirection;

    public void SetMovementVector(Vector2 movementVector)
    {
        MovementVector = movementVector;

        if (IsWalking)
        {
            if (movementVector.Y > 0)
            {
                FacingDirection = Direction.Down;
            }
            else if (movementVector.Y < 0)
            {
                FacingDirection = Direction.Up;
            }
            else if (movementVector.X > 0)
            {
                FacingDirection = Direction.Right;
            }
            else if (movementVector.X < 0)
            {
                FacingDirection = Direction.Left;
            }
        }
    }

    public RectangleF GetInteractionBox()
    {
        var collisionBox = GetCollisionBox();
        var interactionSize = new Size2(collisionBox.Width, 5F);

        var direction = GetFacingDirection();
        var interactionBox = direction switch
        {
            Direction.Up => new RectangleF(new Point2(collisionBox.X, collisionBox.Y - interactionSize.Height),
                interactionSize),
            Direction.Down => new RectangleF(new Point2(collisionBox.X, collisionBox.Bottom), interactionSize),
            Direction.Left => new RectangleF(new Point2(collisionBox.X - interactionSize.Width, collisionBox.Y),
                interactionSize),
            _ => new RectangleF(new Point2(collisionBox.Right, collisionBox.Y),
                interactionSize)
        };

        return interactionBox;
    }


    public void Update(GameTime gameTime)
    {
        UpdateAnimation(gameTime);
        InvulnerabilityTimer.Update(gameTime);
        KnockBackLerper.Update(gameTime);
    }

    public void UpdatePaused(GameTime gameTime)
    {
        UpdateAnimation(gameTime);
    }

    private void UpdateAnimation(GameTime gameTime)
    {
        if (HitPoints <= 0)
        {
            CurrentAnimation = DeathAnimation;
        }

        if (IsAttacking && CurrentAnimation.IsAnimationOver())
        {
            IsAttacking = false;
        }

        PreviousFacingDirection = CurrentFacingDirection;
        CurrentFacingDirection = FacingDirection;

        PreviousIsAttacking = CurrentIsAttacking;
        CurrentIsAttacking = IsAttacking;

        PreviousIsWalking = CurrentIsWalking;
        CurrentIsWalking = IsWalking;

        var attackChanged = CurrentIsAttacking != PreviousIsAttacking;
        var walkChanged = CurrentIsWalking != PreviousIsWalking;
        var directionChanged = CurrentFacingDirection != PreviousFacingDirection;

        if (attackChanged || walkChanged || directionChanged)
        {
            SetAnimation();
        }

        CurrentAnimation.Update(gameTime);

        if (IsAttacking)
        {
            CalculateAttackRectangle();
        }
    }

    private void SetAnimation()
    {
        CurrentAnimation.Reset();
        if (CurrentIsAttacking)
        {
            CurrentAnimation = CurrentFacingDirection switch
            {
                Direction.Down => AttackingSouthAnimation,
                Direction.Up => AttackingNorthAnimation,
                _ => AttackingSideAnimation
            };
        }
        else if (CurrentIsWalking)
        {
            CurrentAnimation = CurrentFacingDirection switch
            {
                Direction.Down => WalkingSouthAnimation,
                Direction.Up => WalkingNorthAnimation,
                _ => WalkingSideAnimation
            };
        }
        else
        {
            CurrentAnimation = CurrentFacingDirection switch
            {
                Direction.Down => IdleSouthAnimation,
                Direction.Up => IdleNorthAnimation,
                _ => IdleSideAnimation
            };
        }
    }

    private void CalculateAttackRectangle()
    {
        var animFrame = CurrentAnimation.GetCurrentFrame();
        var pPos = Position.ToPoint();

        if (FacingDirection == Direction.Left)
        {
            HitBox = animFrame switch
            {
                0 => new Rectangle(new Point(0, 0), new Point(0, 0)),
                1 => new Rectangle(new Point(pPos.X + 6, pPos.Y + 33), new Point(13, 14)),
                2 => new Rectangle(0, 0, 0, 0),
                3 => new Rectangle(0, 0, 0, 0),
                _ => HitBox
            };
        }
        else if (FacingDirection == Direction.Right)
        {
            HitBox = animFrame switch
            {
                0 => new Rectangle(new Point(0, 0), new Point(0, 0)),
                1 => new Rectangle(new Point(pPos.X + 32, pPos.Y + 32), new Point(13, 14)),
                2 => new Rectangle(0, 0, 0, 0),
                3 => new Rectangle(0, 0, 0, 0),
                _ => HitBox
            };
        }
        else if (FacingDirection == Direction.Down)
        {
            HitBox = animFrame switch
            {
                0 => new Rectangle(new Point(0, 0), new Point(0, 0)),
                1 => new Rectangle(pPos.X + 17, pPos.Y + 37, 22, 11),
                2 => new Rectangle(0, 0, 0, 0),
                3 => new Rectangle(0, 0, 0, 0),
                _ => HitBox
            };
        }
        else if (FacingDirection == Direction.Up)
        {
            HitBox = animFrame switch
            {
                0 => new Rectangle(new Point(0, 0), new Point(0, 0)),
                1 => new Rectangle(pPos.X + 11, pPos.Y + 22, 22, 11),
                2 => new Rectangle(0, 0, 0, 0),
                3 => new Rectangle(0, 0, 0, 0),
                _ => HitBox
            };
        }
    }

    private Rectangle GetSourceRectangle()
    {
        return CurrentAnimation.GetSourceRectangle();
    }

    public bool IsDead() => HitPoints <= 0;
    public bool IsDeathAnimationOver() => DeathAnimation.IsAnimationOver();

    public RectangleF GetHitBox() => HitBox;
}