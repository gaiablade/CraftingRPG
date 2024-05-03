using System;
using System.Collections.Generic;
using System.Linq;
using CraftingRPG.AssetManagement;
using CraftingRPG.Entities;
using CraftingRPG.Enums;
using CraftingRPG.Extensions;
using CraftingRPG.Global;
using CraftingRPG.Graphics;
using CraftingRPG.InputManagement;
using CraftingRPG.Interfaces;
using CraftingRPG.Lerpers;
using CraftingRPG.MapManagement;
using CraftingRPG.SoundManagement;
using CraftingRPG.SourceRectangleProviders;
using CraftingRPG.Timers;
using CraftingRPG.Utility;
using Microsoft.Xna.Framework;

// I love you

namespace CraftingRPG.GameStateManagement.GameStates;

public class OverWorldGameState : BaseGameState
{
    private OverWorldState CurrentState = OverWorldState.ControllingPlayer;

    // References
    private readonly PlayerInstance Player;

    // Dropped item label
    private ItemLabel Label;

    // Used to prevent enemies from being attacked multiple times in the same swing
    private readonly List<IEnemyInstance> AttackedEnemies = new();

    // Game over
    private ILerper<float> GameOverFadeLerper;

    // Keybinding display
    private KeybindingDisplayState KeybindingState = KeybindingDisplayState.Displayed;
    private readonly ITimer KeybindingDisplayTimer;
    private readonly ITimer KeybindingSlideTimer;
    private readonly ISourceRectangleProvider<InputAction> InputActionSourceRectangleProvider;

    public OverWorldGameState()
    {
        Player = Globals.Player;
        Player.Position = new Vector2(100, 100);

        MapManager.Instance.LoadDefaultMap();
        SoundManager.Instance.PlaySong(Assets.Instance.Field02, loop: true, volume: 0.5F);

        Label = new ItemLabel();

        KeybindingDisplayTimer = new LinearTimer(3);
        KeybindingSlideTimer = new EaseOutTimer(2);
        InputActionSourceRectangleProvider = new InputActionKeySourceRectangleProvider();
    }

    public override void DrawWorld()
    {
        DrawMap();
    }

    public override void DrawUi()
    {
        MapManager.Instance.DrawUi();

        var windowBounds = GameManager.WindowBounds;
        var woodUi = Assets.Instance.WoodUiSpriteSheet;

        var position = (int)Label.Lerper.GetLerpedValue();

        GameManager.SpriteBatch.Draw(woodUi,
            new Rectangle(new Point(windowBounds.Center.X - Label.Bounds.Center.X, position), Label.Size),
            Label.SourceRectangle,
            Color.White);

        var itemNameData = Assets.Instance.Monogram24.GetDrawingData(Label.ItemName);
        GameManager.SpriteBatch.DrawTextDrawingData(itemNameData,
            new Vector2(windowBounds.Center.X - itemNameData.Dimensions.X / 2, 25 + position),
            Color.Black);

        if (KeybindingState != KeybindingDisplayState.Gone)
        {
            var drawingData = new
            {
                TextDrawingData = Assets.Instance.Monogram24.GetDrawingData("Keybindings"),
                KeyDrawingData = new SpriteDrawingData
                {
                    Texture = Assets.Instance.KeyIconSpriteSheet,
                    SourceRectangle = InputActionSourceRectangleProvider.GetSourceRectangle(InputAction.OpenKeybindings)
                }
            };

            var width = drawingData.KeyDrawingData.SourceRectangle.Width + 20 +
                        drawingData.TextDrawingData.Dimensions.X;
            var height = drawingData.KeyDrawingData.SourceRectangle.Height + 5;

            var x = GameManager.WindowBounds.Right - width + width * (float)KeybindingSlideTimer.GetPercent();
            var y = GameManager.WindowBounds.Bottom - height;
            GameManager.SpriteBatch.Draw(drawingData.KeyDrawingData.Texture,
                new Vector2(x, y),
                drawingData.KeyDrawingData.SourceRectangle,
                Color.White);
            GameManager.SpriteBatch.DrawTextDrawingData(drawingData.TextDrawingData,
                new Vector2(x + 10 + drawingData.KeyDrawingData.SourceRectangle.Width, y),
                Color.FloralWhite);
        }

        if (CurrentState != OverWorldState.PlayerDead) return;

        var opacity = GameOverFadeLerper.GetLerpedValue();
        GameManager.SpriteBatch.Draw(GameManager.Pixel,
            GameManager.WindowBounds,
            Color.Black * opacity);
    }

    public override void Update(GameTime gameTime)
    {
        switch (KeybindingState)
        {
            case KeybindingDisplayState.Displayed:
                KeybindingDisplayTimer.Update(gameTime);
                if (KeybindingDisplayTimer.IsDone())
                {
                    KeybindingState = KeybindingDisplayState.Sliding;
                }

                break;
            case KeybindingDisplayState.Sliding:
                KeybindingSlideTimer.Update(gameTime);
                if (KeybindingSlideTimer.IsDone())
                {
                    KeybindingState = KeybindingDisplayState.Gone;
                }

                break;
            case KeybindingDisplayState.Gone:
            default:
                break;
        }

        switch (CurrentState)
        {
            case OverWorldState.ControllingPlayer:
            {
                Label.Lerper.Update(gameTime);

                if (MapManager.Instance.IsMapTransitioning())
                {
                    HandleInstanceUpdates(gameTime);
                    MapManager.Instance.Update(gameTime);
                    HandleItemLabel();
                    return;
                }

                CalculateMovement();
                DetectCollisionsAndPerformMovement();
                HandlePickup();
                HandleAttacks();
                HandleInstanceUpdates(gameTime);

                MapManager.Instance.Update(gameTime);

                HandleItemLabel();
                CheckForMenuOpened();

                break;
            }
            case OverWorldState.PlayerDead:
            {
                HandleInstanceUpdates(gameTime);
                HandleGameOverCondition();
                if (Player.IsDeathAnimationOver())
                {
                    GameOverFadeLerper.Update(gameTime);
                }

                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static void DrawMap()
    {
        MapManager.Instance.Draw(GameManager.SpriteBatch);
    }

    private void CalculateMovement()
    {
        // Reset movement vector
        var movementVector = Vector2.Zero;

        // If player is attacking or getting knocked back, we should ignore movement
        if (!Player.IsAttacking)
        {
            CalculateInputMovementVector(ref movementVector);

            if (movementVector != Vector2.Zero)
            {
                Player.IsWalking = true;
                movementVector = CustomMath.UnitVector(movementVector);
                movementVector = Vector2.Multiply(movementVector, PlayerInstance.MovementSpeed * (float)Time.Delta);
            }
            else
            {
                Player.IsWalking = false;
            }
        }

        Player.SetMovementVector(movementVector);
    }

    private static void CalculateInputMovementVector(ref Vector2 movementVector)
    {
        if (InputManager.Instance.GetDurationHeld(InputAction.MoveEast) > 0)
        {
            movementVector.X = 1;
        }
        else if (InputManager.Instance.GetDurationHeld(InputAction.MoveWest) > 0)
        {
            movementVector.X = -1;
        }

        if (InputManager.Instance.GetDurationHeld(InputAction.MoveNorth) > 0)
        {
            movementVector.Y = -1;
        }
        else if (InputManager.Instance.GetDurationHeld(InputAction.MoveSouth) > 0)
        {
            movementVector.Y = 1;
        }
    }

    private static void DetectCollisionsAndPerformMovement()
    {
        var movingInstances = GetMovingInstances();

        foreach (var instance in movingInstances)
        {
            var movementVector = instance.GetMovementVector();
            var projectedCollider = instance.GetCollisionBox();
            projectedCollider.X += movementVector.X;
            projectedCollider.Y += movementVector.Y;

            // Check for collision with map objects
            foreach (var objectLayer in MapManager.Instance.GetObjectLayers())
            {
                foreach (var mapObject in objectLayer.Objects)
                {
                    var isSolid = mapObject.Attributes.IsSolid;
                    var objectCollider = mapObject.GetCollisionBox();

                    if (!isSolid || !projectedCollider.Intersects(objectCollider)) continue;

                    var collisionDepth = projectedCollider.GetIntersectionDepth(objectCollider);
                    var absoluteDepth = new Vector2(Math.Abs(collisionDepth.X), Math.Abs(collisionDepth.Y));

                    if (absoluteDepth.Y < absoluteDepth.X)
                    {
                        movementVector.Y += collisionDepth.Y;
                    }
                    else
                    {
                        movementVector.X += collisionDepth.X;
                    }
                }
            }

            instance.Move(movementVector);
        }
    }

    private void HandlePickup()
    {
        if (!Player.IsAboveDrop || !InputManager.Instance.IsKeyPressed(InputAction.Attack)) return;

        InputManager.Instance.Debounce(InputAction.Attack);
        var drop = Player.DropsBelowPlayer.First();
        drop.OnObtain();
        Player.DropsBelowPlayer.Remove(drop);
        MapManager.Instance.RemoveDrop(drop);
    }

    private void HandleAttacks()
    {
        HandlePlayerAttacks();
        HandleEnemyAttacks();
        HandleDefeatedEnemies();
    }

    private void HandlePlayerAttacks()
    {
        var enemies = MapManager.Instance.GetEnemyInstances();

        // Is player attacking enemies?
        if (Player.IsAttacking)
        {
            foreach (var enemy in enemies)
            {
                if (AttackedEnemies.Contains(enemy) ||
                    !enemy.GetCollisionBox().Intersects(Player.GetHitBox())) continue;

                Assets.Instance.HitSfx01.Play(0.3F, 0F, 0F);
                var damage = Player.Info.Equipment.Weapon.GetAttackStat();
                enemy.IncurDamage(damage);

                var enemyCenter = enemy.GetCollisionBox().Center;
                var playerCenter = Player.GetCollisionBox().Center;
                var knockBackAngle =
                    CustomMath.UnitVector(Vector2.Subtract(enemyCenter, playerCenter));
                var knockBackPosition = Vector2.Add(enemy.GetPosition(), Vector2.Multiply(knockBackAngle, 25));
                enemy.SetKnockBack(new LinearVector2Lerper(enemy.GetPosition(), knockBackPosition, 0.1));
                AttackedEnemies.Add(enemy);
            }
        }
        else
        {
            AttackedEnemies.Clear();
            if (!InputManager.Instance.IsKeyPressed(InputAction.Attack)) return;
            Player.IsAttacking = true;
            Assets.Instance.SwingSfx01.Play(volume: 0.1F, 0F, 0F);
        }
    }

    private void HandleEnemyAttacks()
    {
        var enemies = MapManager.Instance.GetEnemyInstances();

        // Are enemies attacking the player?
        if (!Player.InvulnerabilityTimer.IsDone()) return;

        foreach (var enemy in enemies)
        {
            if (!enemy.IsAttacking()) continue;

            var hitBox = enemy.GetAttackHitBox();
            if (!Player.GetCollisionBox().Intersects(hitBox)) continue;

            // Player is hit!
            var enemyInfo = enemy.GetEnemyInfo();
            Player.HitPoints -= enemyInfo.GetAttackDamage();
            Assets.Instance.Impact01.Play(0.4F, 0F, 0F);

            Player.InvulnerabilityTimer.Reset();

            if (!Player.IsDead())
            {
                var knockBackAngle = enemy.GetAttackAngle();
                var knockBackPosition = Vector2.Add(Player.Position, Vector2.Multiply(knockBackAngle, 25));
                Player.KnockBackLerper = new LinearVector2Lerper(Player.Position, knockBackPosition, 0.1);
            }
            else
            {
                SetState(OverWorldState.PlayerDead);
            }
        }
    }

    private static void HandleDefeatedEnemies()
    {
        var enemies = MapManager.Instance.GetEnemyInstances();
        var defeatedEnemies = GetDefeatedEnemies(enemies);

        foreach (var enemy in defeatedEnemies)
        {
            var enemyInfo = enemy.GetEnemyInfo();

            enemy.OnDeath();

            foreach (var quest in Globals.Player.Info.QuestBook.GetDefeatEnemyQuests())
            {
                quest.OnEnemyDefeated(enemyInfo);
            }

            var dropTable = enemyInfo.GetDropTable();
            foreach (var possibleDrop in dropTable)
            {
                var randomNumber = Random.Shared.Next() % 100;

                if (randomNumber >= possibleDrop.DropRate) continue;

                var dropInstance = possibleDrop.CreateDropInstance();

                if (!dropInstance.CanDrop())
                {
                    continue;
                }

                var dropPosition = Vector2.Subtract(enemy.GetCollisionBox().Center, new Vector2(16, 16));
                dropInstance.SetPosition(dropPosition);

                MapManager.Instance.AddDrop(dropInstance);
            }

            enemies.Remove(enemy);
        }
    }

    private static IEnumerable<IEnemyInstance> GetDefeatedEnemies(IEnumerable<IEnemyInstance> enemies)
    {
        return enemies.Where(enemy => enemy.IsDefeated()).ToList();
    }

    private void HandleInstanceUpdates(GameTime gameTime)
    {
        Player.Update(gameTime);
        foreach (var enemy in MapManager.Instance.GetEnemyInstances())
        {
            enemy.Update(gameTime);
        }
    }

    private void HandleGameOverCondition()
    {
        if (!Player.IsDead() || !Player.IsDeathAnimationOver() || !GameOverFadeLerper.IsDone()) return;

        SoundManager.Instance.PlaySong(Assets.Instance.GameOver02, false);
        GameStateManager.Instance.PushState(new GameOverGameState());
    }

    private void HandleItemLabel()
    {
        if (Player.IsAboveDrop)
        {
            Label.ItemName = Player.DropsBelowPlayer.First().GetDroppable().GetName();
        }

        if (Player.IsAboveDrop && !Label.PreviousIsAboveDrop)
        {
            Label.Lerper = new EaseOutDoubleLerper(Label.Lerper.GetLerpedValue(), 0, ItemLabel.DropTime);
        }
        else if (!Player.IsAboveDrop && Label.PreviousIsAboveDrop)
        {
            Label.Lerper = new EaseOutDoubleLerper(Label.Lerper.GetLerpedValue(), -64, ItemLabel.DropTime);
        }

        Label.PreviousIsAboveDrop = Player.IsAboveDrop;
    }

    private static void CheckForMenuOpened()
    {
        if (InputManager.Instance.IsKeyPressed(InputAction.OpenCraftingMenu))
        {
            GameStateManager.Instance.PushState<CraftingMenuGameState>(true);
        }
        else if (InputManager.Instance.IsKeyPressed(InputAction.OpenInventoryMenu))
        {
            GameStateManager.Instance.PushState<InventoryGameState>(true);
        }
        else if (InputManager.Instance.IsKeyPressed(InputAction.OpenQuestsMenu))
        {
            GameStateManager.Instance.PushState<QuestMenuGameState>(true);
        }
        else if (InputManager.Instance.IsKeyPressed(InputAction.OpenKeybindings))
        {
            GameStateManager.Instance.PushState<KeybindingsGameState>(true);
        }
    }

    private static IEnumerable<IInstance> GetMovingInstances()
    {
        var movingInstances = new List<IInstance>();
        if (Globals.Player.GetMovementVector() != Vector2.Zero)
        {
            movingInstances.Add(Globals.Player);
        }

        movingInstances.AddRange(MapManager.Instance.GetEnemyInstances()
            .Where(enemy => enemy.GetMovementVector() != Vector2.Zero));

        return movingInstances;
    }

    private void SetState(OverWorldState state)
    {
        switch (state)
        {
            case OverWorldState.ControllingPlayer:
                break;
            case OverWorldState.PlayerDead:
                CurrentState = OverWorldState.PlayerDead;
                SoundManager.Instance.FadeOut(2.0);
                GameOverFadeLerper = new LinearFloatLerper(0F, 1F, 1.5);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private enum OverWorldState
    {
        ControllingPlayer,
        PlayerDead
    }

    private enum KeybindingDisplayState
    {
        Displayed,
        Sliding,
        Gone
    }

    private struct ItemLabel
    {
        public const double DropTime = 0.4;
        public readonly Point Size = new Point(64 * 3, 64);
        public readonly Rectangle SourceRectangle = new(416, 96, 32 * 3, 32);
        public readonly Rectangle Bounds;
        public bool PreviousIsAboveDrop;
        public ILerper<double> Lerper;
        public string ItemName;

        public ItemLabel()
        {
            ItemName = string.Empty;
            Bounds = new Rectangle(Point.Zero, Size);
            PreviousIsAboveDrop = false;
            Lerper = new EaseOutDoubleLerper(0, -64, DropTime);
            Lerper.SetTime(0.4);
        }
    }
}