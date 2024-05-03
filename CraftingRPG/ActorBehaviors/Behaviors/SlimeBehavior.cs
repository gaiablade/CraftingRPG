using System;
using CraftingRPG.AssetManagement;
using CraftingRPG.Global;
using CraftingRPG.Interfaces;
using CraftingRPG.SpriteAnimation;
using CraftingRPG.Timers;
using CraftingRPG.Utility;
using Microsoft.Xna.Framework;

namespace CraftingRPG.ActorBehaviors.Behaviors;

public enum SlimeActorBehaviorState
{
    None,
    Idle,
    FollowingPath,
    MovingTowardsPlayer,
    Attacking,
    AttackCooldown,
    Dying
}

public class SlimeBehavior : BaseBehavior
{
    private const float LineOfSightLength = 100F;

    private Vector2 Position;
    private Vector2 Center => Vector2.Add(Position, new Vector2(16, 16));
    private Vector2 MovementVector = Vector2.Zero;
    private SlimeActorBehaviorState BehaviorState = SlimeActorBehaviorState.Idle;

    private ITimer IdleTimer;

    private Vector2 MovementDirection;
    private ITimer MovementTimer;

    // Animations
    private Animation IdleAnimation;
    private Animation MovingAnimation;
    private Animation AttackAnimation;
    private Animation DeathAnimation;

    public Animation CurrentAnimation { get; private set; }

    // Attacking
    private bool AttackSoundPlayed;
    private Vector2 AttackTarget = Vector2.Zero;
    private BehaviorPath AttackPath;
    private ITimer AttackTimer;
    private Vector2 AttackAngle;

    // KnockBack
    private ILerper<Vector2> KnockBackPath;
    private bool IsKnockedBack;

    public SlimeBehavior()
    {
        IdleTimer = new LinearTimer(1);
        MovementTimer = new LinearTimer(3.0);

        IdleAnimation = new Animation(4, 0.2, new Point(32, 32));
        MovingAnimation = new Animation(6, 0.1, new Point(32, 32), true, 0, 32);
        AttackAnimation = new Animation(7, 0.14, new Point(32, 32), false, 0, 64);
        DeathAnimation = new Animation(6, 0.1, new Point(32, 32), false, 0, 128);

        CurrentAnimation = IdleAnimation;
    }

    public SlimeActorBehaviorState GetBehaviorState() => BehaviorState;

    public override void SetPosition(Vector2 position)
    {
        Position = position;
    }

    public override Vector2 GetPosition() => Position;
    public Vector2 GetMovementVector() => MovementVector;

    public override void Update(GameTime gameTime)
    {
        var player = Globals.Player;
        CurrentAnimation.Update(gameTime);

        var isPlayerInLos = Vector2.Distance(player.Position, Position) <= LineOfSightLength;
        MovementVector = Vector2.Zero;

        if (isPlayerInLos && !player.IsDead() && BehaviorState != SlimeActorBehaviorState.Dying &&
            BehaviorState != SlimeActorBehaviorState.Attacking &&
            BehaviorState != SlimeActorBehaviorState.MovingTowardsPlayer &&
            BehaviorState != SlimeActorBehaviorState.AttackCooldown)
        {
            SetState(SlimeActorBehaviorState.MovingTowardsPlayer);
        }

        // State Machine
        if (BehaviorState == SlimeActorBehaviorState.None)
        {
            SetState(SlimeActorBehaviorState.Idle);
        }
        else if (BehaviorState == SlimeActorBehaviorState.Idle)
        {
            IdleTimer.Update(gameTime);
            if (!IdleTimer.IsDone()) return;

            SetState(SlimeActorBehaviorState.FollowingPath);
        }
        else if (BehaviorState == SlimeActorBehaviorState.FollowingPath)
        {
            MovementTimer.Update(gameTime);

            if (!MovementTimer.IsDone())
            {
                const float speed = 30F;
                var movementVector = Vector2.Multiply(MovementDirection, speed * (float)Time.Delta);

                MovementVector = movementVector;
            }
            else
            {
                SetState(SlimeActorBehaviorState.Idle);
            }
        }
        else if (BehaviorState == SlimeActorBehaviorState.MovingTowardsPlayer)
        {
            // If player is out of range, go back to idling
            if (player.IsDead() || Vector2.Distance(player.Center, Center) > LineOfSightLength)
            {
                SetState(SlimeActorBehaviorState.Idle);
                return;
            }

            const float attackDistance = 50F;
            var distance = Vector2.Distance(Position, Globals.Player.Center);
            if (distance <= attackDistance)
            {
                SetState(SlimeActorBehaviorState.Attacking);
                return;
            }

            const float speed = 30F;
            var unitVector = CustomMath.UnitVector(Vector2.Subtract(Globals.Player.Center, Center));
            var movementVector = Vector2.Multiply(unitVector, speed * (float)Time.Delta);
            MovementVector = movementVector;
        }
        else if (BehaviorState == SlimeActorBehaviorState.Attacking)
        {
            if (AttackAnimation.GetCurrentFrame() > 1)
            {
                if (!AttackSoundPlayed)
                {
                    Assets.Instance.Swoosh02.Play(0.3F, 0F, 0F);
                    AttackSoundPlayed = true;
                }

                if (!IsKnockedBack)
                {
                    AttackTimer.Update(gameTime);
                    var percent = AttackTimer.GetPercent();
                    var targetPoint = Vector2.Lerp(AttackPath.Start, AttackPath.End, (float)percent);
                    // Target point is for center, so need to adjust to top left
                    targetPoint = Vector2.Subtract(targetPoint, new Vector2(16, 16));

                    MovementVector = Vector2.Subtract(targetPoint, Position);
                }
            }
            else
            {
                AttackTarget = player.Center;
                AttackPath = new BehaviorPath
                {
                    Start = Center,
                    End = AttackTarget
                };
            }

            if (AttackAnimation.GetCurrentFrame() == 6)
            {
                SetState(SlimeActorBehaviorState.AttackCooldown);
            }
        }
        else if (BehaviorState == SlimeActorBehaviorState.AttackCooldown)
        {
            IdleTimer.Update(gameTime);
            if (IdleTimer.IsDone())
            {
                SetState(SlimeActorBehaviorState.MovingTowardsPlayer);
            }
        }
        else if (BehaviorState == SlimeActorBehaviorState.Dying)
        {
        }

        // Regardless of current state, apply knockback
        if (KnockBackPath != null && !KnockBackPath.IsDone())
        {
            KnockBackPath.Update(gameTime);
            MovementVector = Vector2.Subtract(KnockBackPath.GetLerpedValue(), Position);
        }
    }

    private void SetState(SlimeActorBehaviorState state)
    {
        BehaviorState = state;
        switch (state)
        {
            case SlimeActorBehaviorState.Idle:
                IdleAnimation.Reset();
                var idleCycles = Random.Shared.Next(5);
                IdleTimer = new LinearTimer(idleCycles * IdleAnimation.GetDuration());
                CurrentAnimation = IdleAnimation;
                break;
            case SlimeActorBehaviorState.FollowingPath:
                MovingAnimation.Reset();
                CurrentAnimation = MovingAnimation;
                var distance = Random.Shared.Next(4) * MovingAnimation.GetDuration();
                var angle = Random.Shared.Next(360) * Math.PI / 180;
                MovementDirection = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                MovementTimer = new LinearTimer(distance);
                break;
            case SlimeActorBehaviorState.MovingTowardsPlayer:
                MovingAnimation.Reset();
                CurrentAnimation = MovingAnimation;
                break;
            case SlimeActorBehaviorState.Attacking:
                IsKnockedBack = false;
                AttackAnimation.Reset();
                CurrentAnimation = AttackAnimation;
                AttackTarget = Globals.Player.Center;
                AttackPath = new BehaviorPath
                {
                    Start = Center,
                    End = AttackTarget
                };
                AttackAngle = CustomMath.UnitVector(Vector2.Subtract(AttackTarget, Center));
                AttackTimer = new LinearTimer(0.14 * 3);
                AttackSoundPlayed = false;
                break;
            case SlimeActorBehaviorState.AttackCooldown:
                IsKnockedBack = false;
                IdleAnimation.Reset();
                CurrentAnimation = IdleAnimation;
                KnockBackPath = null;
                var duration = (Random.Shared.Next(2) + 2) * IdleAnimation.GetDuration();
                IdleTimer = new LinearTimer(duration);
                break;
            case SlimeActorBehaviorState.Dying:
                DeathAnimation.Reset();
                CurrentAnimation = DeathAnimation;
                break;
        }
    }

    public Rectangle GetAttackHitBox()
    {
        switch (AttackAnimation.GetCurrentFrame())
        {
            case 0:
            case 1:
                return Rectangle.Empty;
            case 2:
                return new Rectangle(Vector2.Add(Position, new Vector2(10, 4)).ToPoint(), new Point(12, 13));
            case 3:
                return new Rectangle(Vector2.Add(Position, new Vector2(10, 2)).ToPoint(), new Point(12, 12));
            case 4:
                return new Rectangle(Vector2.Add(Position, new Vector2(9, 4)).ToPoint(), new Point(13, 12));
            case 5:
                return new Rectangle(Vector2.Add(Position, new Vector2(8, 14)).ToPoint(), new Point(16, 9));
            case 6:
                return new Rectangle(Vector2.Add(Position, new Vector2(7, 15)).ToPoint(), new Point(18, 8));
        }

        return Rectangle.Empty;
    }

    public Vector2 GetAttackAngle() => AttackAngle;

    public void SetKnockBackPath(ILerper<Vector2> path)
    {
        KnockBackPath = path;
        IsKnockedBack = true;

        // If the enemy is attacking, it should not continue traveling towards its target
        if (BehaviorState == SlimeActorBehaviorState.Attacking)
        {
            AttackPath = new BehaviorPath()
            {
                Start = Position,
                End = path.GetEnd(),
                Current = Position
            };
        }
    }

    public void SetDeathState(bool isDead = true)
    {
        SetState(SlimeActorBehaviorState.Dying);
    }

    public bool IsDeathAnimationOver() => DeathAnimation.IsAnimationOver();
}