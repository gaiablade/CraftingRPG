using CraftingRPG.Entities;
using CraftingRPG.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using CraftingRPG.MapManagement;
using TiledSharp;

// I love you

namespace CraftingRPG.States;

public class OverworldState : IState
{
    private PlayerInstance Player;
    private List<IInstance> MapObjects;
    private List<IDropInstance> Drops;
    private Vector2 MovementVector;
    private bool IsAttacking = false;
    private int AttackAnimFrames = 0;
    private int AttackFrameLength = 8;
    private bool IsWalking = false;
    private int IdleOrWalkingAnimFrames = 0;
    private int AttackFrame = 0;
    private Rectangle AttackRect;
    private List<IEnemyInstance> AttackedEnemies = new();
    private bool IsAboveDrop = false;
    private List<IDropInstance> DropsBelowPlayer;
    private bool ActionKeyPressed = false;

    private GameMap CurrentMap;

    private TmxMap Map;
    private int TileWidth;
    private int TileHeight;

    public OverworldState()
    {
        GameManager.AddKeysIfNotExists(Keys.Left, Keys.Right, Keys.Up, Keys.Down, Keys.Z, Keys.X, Keys.C, Keys.I,
            Keys.Q);

        Global.Globals.Instance.Player = new PlayerInstance(GameManager.PlayerInfo);
        Player = Global.Globals.Instance.Player;
        Player.Position = new Vector2(100, 100);

        MapManager.Instance.LoadDefaultMap();
        CurrentMap = MapManager.Instance.GetCurrentMap();

        MapObjects = new();
        Drops = new();
    }

    public void Render()
    {
        DrawMap();
    }

    private void DrawMap()
    {
        MapManager.Instance.Draw(GameManager.SpriteBatch);
    }

    public void Update(GameTime gameTime)
    {
        MapManager.Instance.Update(gameTime);

        IsAboveDrop = IsPlayerAboveDropInstance(out DropsBelowPlayer);

        if (GameManager.FramesKeysHeld[Keys.C] == 1)
        {
            StateManager.Instance.PushState<CraftingMenuState>(true);
        }
        else if (GameManager.FramesKeysHeld[Keys.I] == 1)
        {
            StateManager.Instance.PushState<InventoryState>(true);
        }
        else if (GameManager.FramesKeysHeld[Keys.Q] == 1)
        {
            StateManager.Instance.PushState<QuestMenuState>(true);
        }
    }

    private bool IsPlayerAboveDropInstance(out List<IDropInstance> drops)
    {
        drops = new List<IDropInstance>();

        var playerBounds = Player.GetBounds();

        foreach (var dropInstance in Drops)
        {
            var pos = dropInstance.GetPosition();
            var dropBounds = new Rectangle((int)pos.X, (int)pos.Y, 32, 32);
            if (dropBounds.Intersects(playerBounds))
            {
                drops.Add(dropInstance);
            }
        }

        return drops.Count > 0;
    }
}