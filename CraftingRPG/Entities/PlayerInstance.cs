using System.Collections.Generic;
using CraftingRPG.AssetManagement;
using CraftingRPG.Constants;
using CraftingRPG.Enums;
using CraftingRPG.Graphics;
using CraftingRPG.Interfaces;
using CraftingRPG.Recipes;
using CraftingRPG.SpriteAnimation;
using CraftingRPG.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Point = Microsoft.Xna.Framework.Point;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using RectangleF = MonoGame.Extended.RectangleF;

namespace CraftingRPG.Entities;

public class PlayerInstance : IInstance
{
    public PlayerInfo Info { get; set; }

    #region Constants
    public const float MovementSpeed = 100F;
    public const int AttackFrameLength = 5;
    public static readonly Point SpriteSize = new Point(48, 48);
    public Point Size = new(48, 48);
    #endregion
    
    #region Public Getters/Setters
    public Vector2 Position = new Vector2();
    public Vector2 Center => Vector2.Add(Position, new Vector2(24, 36));

    public int FacingDirection { get; set; } = Direction.Down;
    public bool IsAttacking { get; set; }
    public bool IsWalking { get; set; }
    public List<IDropInstance> DropsBelowPlayer { get; set; } = new();

    public ITimer InvulnerabilityTimer { get; private set; }
    public int HitPoints { get; set; }
    #endregion
    
    public Vector2 MovementVector;
    public bool IsAboveDrop = false;
    public RectangleF AttackRect;
    
    #region Animation
    private int PreviousFacingDirection;
    private int CurrentFacingDirection = Direction.Down;
    
    private bool PreviousIsAttacking = false;
    private bool CurrentIsAttacking = false;
    
    private bool PreviousIsWalking;
    private bool CurrentIsWalking;
    
    private Animation IdleSouthAnimation;
    private Animation IdleNorthAnimation;
    private Animation IdleSideAnimation;
    private Animation WalkingSouthAnimation;
    private Animation WalkingNorthAnimation;
    private Animation WalkingSideAnimation;
    private Animation AttackingSouthAnimation;
    private Animation AttackingNorthAnimation;
    private Animation AttackingSideAnimation;

    private Animation CurrentAnimation;
    #endregion

    public PlayerInstance(PlayerInfo info)
    {
        const double idleInterval = 0.1;
        const double walkInterval = 0.1;
        const double attackInterval = 0.1;
        
        Info = info;
        IdleSouthAnimation = new Animation(6, idleInterval, SpriteSize, true, 0, 0);
        IdleSideAnimation = new Animation(6, idleInterval, SpriteSize, true, 0, 48);
        IdleNorthAnimation = new Animation(6, idleInterval, SpriteSize, true, 0, 96);
        WalkingSouthAnimation = new Animation(6, walkInterval, SpriteSize, true, 0, 144);
        WalkingSideAnimation = new Animation(6, walkInterval, SpriteSize, true, 0, 192);
        WalkingNorthAnimation = new Animation(6, walkInterval, SpriteSize, true, 0, 240);
        AttackingSouthAnimation = new Animation(4, attackInterval, SpriteSize, false, 0, 288);
        AttackingSideAnimation = new Animation(4, attackInterval, SpriteSize, false, 0, 336);
        AttackingNorthAnimation = new Animation(4, attackInterval, SpriteSize, false, 0, 384);
        CurrentAnimation = IdleSouthAnimation;
        InvulnerabilityTimer = new LinearTimer(1.0);
        InvulnerabilityTimer.Set(1.0);
        HitPoints = Info.Stats.MaxHitPoints;
        
        Info.RecipeBook.AddRecipe(RecipeId.IronSword, new IronSwordRecipe());
        Info.RecipeBook.AddRecipe(RecipeId.MageBracelet, new MageBraceletRecipe());

        Info.Inventory[ItemId.HealingMushroom]++;
        Info.Inventory[ItemId.IronSword]++;
        Info.Inventory[ItemId.EmptyBottle]++;
        Info.Inventory[ItemId.MageBracelet]++;
        Info.Inventory[ItemId.ArcaneFlower]++;
        Info.Inventory[ItemId.SmallHealthPotion]++;
        Info.Inventory[ItemId.HeartyFlower]++;
        Info.Inventory[ItemId.IronHelmet]++;
        Info.Inventory[ItemId.IronChunk]++;
        Info.Inventory[ItemId.MediumHealthPotion]++;
    }

    public Vector2 GetPosition() => Position;

    public Point GetSize() => Size;
    public Rectangle GetBounds() => new Rectangle((int)Position.X, (int)Position.Y, (int)GetSize().X, (int)GetSize().Y);

    public RectangleF GetCollisionBox() => new(new Point2(Position.X + 18, Position.Y + 22), new Size2(13, 19));
    public SpriteDrawingData GetDrawingData()
    {
        return new SpriteDrawingData
        {
            Texture = GetSpriteSheet(),
            SourceRectangle = GetSourceRectangle(),
            Flip = FacingDirection == Direction.Left
        };
    }

    public Texture2D GetSpriteSheet()
    {
        return Assets.Instance.PlayerSpriteSheet;
    }

    public Rectangle GetTextureRectangle()
    {
        return CurrentAnimation.GetSourceRectangle();
    }

    public Vector2 GetMovementVector() => MovementVector;

    public Vector2 SetPosition(Vector2 position) => Position = position;

    public void UpdateAnimation(GameTime gameTime)
    {
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
            AttackRect = animFrame switch
            {
                0 => new Rectangle(new Point(0, 0), new Point(0, 0)),
                1 => new Rectangle(new Point(pPos.X + 6, pPos.Y + 33), new Point(13, 14)),
                2 => new Rectangle(0, 0, 0, 0),
                3 => new Rectangle(0, 0, 0, 0),
                _ => AttackRect
            };
        }
        else if (FacingDirection == Direction.Right)
        {
            AttackRect = animFrame switch
            {
                0 => new Rectangle(new Point(0, 0), new Point(0, 0)),
                1 => new Rectangle(new Point(pPos.X + 32, pPos.Y + 32), new Point(13, 14)),
                2 => new Rectangle(0, 0, 0, 0),
                3 => new Rectangle(0, 0, 0, 0),
                _ => AttackRect
            };
        }
        else if (FacingDirection == Direction.Down)
        {
            AttackRect = animFrame switch
            {
                0 => new Rectangle(new Point(0, 0), new Point(0, 0)),
                1 => new Rectangle(pPos.X + 17, pPos.Y + 37, 22, 11),
                2 => new Rectangle(0, 0, 0, 0),
                3 => new Rectangle(0, 0, 0, 0),
                _ => AttackRect
            };
        }
        else if (FacingDirection == Direction.Up)
        {
            AttackRect = animFrame switch
            {
                0 => new Rectangle(new Point(0, 0), new Point(0, 0)),
                1 => new Rectangle(pPos.X + 11, pPos.Y + 22, 22, 11),
                2 => new Rectangle(0, 0, 0, 0),
                3 => new Rectangle(0, 0, 0, 0),
                _ => AttackRect
            };
        }
    }

    private Rectangle GetSourceRectangle()
    {
        return CurrentAnimation.GetSourceRectangle();
    }

    public double GetDepth()
    {
        return Position.Y + GetSize().Y;
    }
}