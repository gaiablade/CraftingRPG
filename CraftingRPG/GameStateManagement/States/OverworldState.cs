using System;
using System.Linq;
using CraftingRPG.AssetManagement;
using CraftingRPG.Entities;
using CraftingRPG.Enums;
using CraftingRPG.Global;
using CraftingRPG.InputManagement;
using CraftingRPG.Interfaces;
using CraftingRPG.MapManagement;
using CraftingRPG.SoundManagement;
using CraftingRPG.Timers;
using Microsoft.Xna.Framework;

// I love you

namespace CraftingRPG.GameStateManagement.States;

public class OverworldState : BaseState
{
    private PlayerInstance Player;

    // Dropped item label
    private int DroppedItemLabelY = -64;
    private bool PreviousIsAboveDrop = false;
    private ITimer DroppedItemLabelTimer;

    public OverworldState()
    {
        Globals.Player = new PlayerInstance(GameManager.PlayerInfo);
        Player = Globals.Player;
        Player.Position = new Vector2(100, 100);

        MapManager.Instance.LoadDefaultMap();
        SoundManager.Instance.PlaySong(Assets.Instance.Field02, true, 0.5F);
        
        DroppedItemLabelTimer = new EaseOutTimer(0.4, true);
        DroppedItemLabelTimer.Update(new GameTime(TimeSpan.Zero, TimeSpan.MaxValue));
    }

    public override void DrawWorld()
    {
        DrawMap();
    }

    public override void DrawUI()
    {
        MapManager.Instance.DrawUI();
        
        var player = Globals.Player;
        var resolution = GameManager.Resolution;
        var woodUi = Assets.Instance.WoodUISpriteSheet;
        
        var percent = DroppedItemLabelTimer.GetPercent();
        var displacement = 64 * percent;

        var dropLabelWidth = 64 * 3;

        GameManager.SpriteBatch.Draw(woodUi,
            new Rectangle(GameManager.Resolution.X / 2 - dropLabelWidth / 2, DroppedItemLabelY + (int)displacement,
                dropLabelWidth, 64),
            new Rectangle(416, 96, 32 * 3, 32),
            Color.White);

        if (!player.IsAboveDrop) return;

        var dropName = player.DropsBelowPlayer.First().GetDroppable().GetName();
        var dropNameSize = Assets.Instance.Monogram12.MeasureString(dropName);

        GameManager.SpriteBatch.DrawString(Assets.Instance.Monogram12,
            dropName,
            new Vector2(resolution.X / 2 - dropNameSize.X / 2, (float)(25 - 64 + displacement)),
            Color.Black);
    }

    public override void Update(GameTime gameTime)
    {
        DroppedItemLabelTimer.Update(gameTime);

        MapManager.Instance.Update(gameTime);

        var player = Globals.Player;

        if (player.IsAboveDrop && !PreviousIsAboveDrop)
        {
            DroppedItemLabelTimer.SetReverse(false);
        }
        else if (!player.IsAboveDrop && PreviousIsAboveDrop)
        {
            DroppedItemLabelTimer.SetReverse();
        }

        PreviousIsAboveDrop = player.IsAboveDrop;

        if (InputManager.Instance.IsKeyPressed(InputAction.OpenCraftingMenu))
        {
            GameStateManager.Instance.PushState<CraftingMenuState>(true);
        }
        if (InputManager.Instance.IsKeyPressed(InputAction.OpenInventoryMenu))
        {
            GameStateManager.Instance.PushState<InventoryState>(true);
        }
        if (InputManager.Instance.IsKeyPressed(InputAction.OpenQuestsMenu))
        {
            GameStateManager.Instance.PushState<QuestMenuState>(true);
        }
    }
    
    private void DrawMap()
    {
        MapManager.Instance.Draw(GameManager.SpriteBatch);
    }
}