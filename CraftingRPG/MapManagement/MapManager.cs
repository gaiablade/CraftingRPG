using System;
using System.Collections.Generic;
using System.Linq;
using CraftingRPG.AssetManagement;
using CraftingRPG.Constants;
using CraftingRPG.Entities;
using CraftingRPG.Extensions;
using CraftingRPG.Global;
using CraftingRPG.Interfaces;
using CraftingRPG.Lerpers;
using CraftingRPG.MapLoaders;
using CraftingRPG.MapObjects;
using CraftingRPG.Player;
using CraftingRPG.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace CraftingRPG.MapManagement;

public class MapManager
{
    public static readonly MapManager Instance = new();

    #region Constants

    private readonly string[] MapsToLoad =
    {
        "Tmx/map1.tmx",
        "Tmx/map2.tmx",
        "Tmx/map3.tmx"
    };

    #endregion

    #region Loaded Maps

    private Dictionary<string, GameMap> Maps = new();
    private GameMap CurrentMap;

    #endregion

    private List<IDropInstance> Drops = new();
    private List<Vector2> DropInitialPositions = new();
    private List<int> DropHoverTimers = new();

    private MapManagerState State = MapManagerState.Normal;
    private MapTransition QueuedMapTransition;
    private bool IgnoreLoadingZone;
    private ILerper<float> TransitionInLerper;

    #region Getters/Setters

    public GameMap GetMap(string key) => Maps[key];
    public GameMap GetCurrentMap() => CurrentMap;
    public IList<IEnemyInstance> GetEnemyInstances() => CurrentMap.Enemies;
    public IList<ObjectLayer> GetObjectLayers() => CurrentMap.ObjectLayers;
    public IList<IDropInstance> GetDrops() => Drops;

    public void RemoveDrop(IDropInstance drop)
    {
        var i = Drops.IndexOf(drop);
        Drops.Remove(drop);
        DropInitialPositions.RemoveAt(i);
        DropHoverTimers.RemoveAt(i);
    }

    #endregion

    #region Public Methods

    public void LoadMapsFromContents(ContentManager contentManager)
    {
        foreach (var mapToLoad in MapsToLoad)
        {
            var map = TiledMapLoader.Instance.LoadMapFromFile(mapToLoad, contentManager);
            Maps[mapToLoad] = map;
        }
    }

    public void LoadDefaultMap()
    {
        SetMap("Tmx/map1.tmx");
    }

    public void SetMap(string mapName)
    {
        CurrentMap = GetMap(mapName);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var tileLayer in CurrentMap.TileLayers)
        {
            DrawTileLayer(spriteBatch, tileLayer);
        }

        var instances = new List<IInstance>();
        instances.Add(Globals.Player);
        instances.AddRange(CurrentMap.Enemies);
        foreach (var objectLayer in CurrentMap.ObjectLayers)
        {
            instances.AddRange(objectLayer.Objects);
        }

        instances.AddRange(Drops);

        // TODO: Remove Linq
        instances = instances.OrderBy(x => x.GetDepth()).ThenBy(x => x.GetPosition().X).ToList();

        foreach (var instance in instances)
        {
            if (instance is PlayerInstance playerInstance)
            {
                DrawPlayer(playerInstance);
            }
            else if (instance is IEnemyInstance enemyInstance)
            {
                var spriteData = enemyInstance.GetDrawingData();
                GameManager.SpriteBatch.Draw(spriteData.Texture,
                    new Rectangle(enemyInstance.GetPosition().ToPoint(), enemyInstance.GetSize()),
                    spriteData.SourceRectangle,
                    Color.White,
                    spriteData.Rotation,
                    spriteData.Origin,
                    spriteData.Flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    0F);

#pragma warning disable CS0162 // Unreachable code detected
                if (Flags.DebugShowEnemyHitBoxes && enemyInstance.IsAttacking())
                {
                    GameManager.SpriteBatch.Draw(GameManager.Pixel,
                        enemyInstance.GetAttackHitBox(),
                        Color.Red);
                }
#pragma warning restore CS0162 // Unreachable code detected
            }

            else if (instance is IMapObject mapObject)
            {
                if (mapObject.GetTileSet() == null) continue;
                GameManager.SpriteBatch.Draw(mapObject.GetTileSet().SpriteSheetTexture,
                    new Rectangle(new Point((int)mapObject.GetPosition().X, (int)mapObject.GetPosition().Y),
                        new Point(mapObject.GetSize().X, mapObject.GetSize().Y)),
                    mapObject.GetTextureRectangle(),
                    Color.White);
            }
            else if (instance is IDropInstance dropInstance)
            {
                var spriteData = dropInstance.GetDrawingData();
                GameManager.SpriteBatch.Draw(spriteData.Texture,
                    new Rectangle(dropInstance.GetPosition().ToPoint(), dropInstance.GetSize()),
                    spriteData.SourceRectangle,
                    Color.White,
                    spriteData.Rotation,
                    spriteData.Origin,
                    spriteData.Flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    0F);
            }
        }
    }

    public void DrawUi()
    {
        var stringData =
            Assets.Instance.Monogram24.GetDrawingData(
                $"{Globals.Player.GetHitPoints()}/{Globals.Player.GetInfo().Stats.MaxHitPoints}");
        GameManager.SpriteBatch.DrawTextDrawingData(stringData,
            new Vector2(10, 10),
            Color.Red);

        var position = Globals.Player.GetPosition().ToPoint();
        stringData = Assets.Instance.Monogram24.GetDrawingData($"({position.X}, {position.Y})");
        GameManager.SpriteBatch.DrawTextDrawingData(stringData,
            new Vector2(10, 30),
            Color.White);

        if (State == MapManagerState.TransitioningOut)
        {
            GameManager.SpriteBatch.Draw(GameManager.Pixel,
                GameManager.WindowBounds,
                Color.Black * (float)QueuedMapTransition.ScreenFadeTimer.GetPercent());
        }
        else if (State == MapManagerState.TransitioningIn)
        {
            GameManager.SpriteBatch.Draw(GameManager.Pixel,
                GameManager.WindowBounds,
                Color.Black * TransitionInLerper.GetLerpedValue());
        }
    }

    public void Update(GameTime gameTime)
    {
        if (State == MapManagerState.Normal)
        {
            CalculateCameraPosition();

            UpdateDrops();
            Globals.Player.SetIsAboveDrop(IsPlayerAboveDropInstance(out var dropsBelowPlayer));
            Globals.Player.SetDropsBelowPlayer(dropsBelowPlayer);

            for (var i = 0; i < Drops.Count - 1; i++)
            {
                var cb1 = Drops[i].GetCollisionBox();
                for (int j = i + 1; j < Drops.Count; j++)
                {
                    var cb2 = Drops[j].GetCollisionBox();
                    if (cb2.Intersects(cb1))
                    {
                        var pos1 = Drops[i].GetPosition();
                        var pos2 = Drops[j].GetPosition();
                        Drops[i].SetPosition(new Vector2(pos1.X - 1, pos1.Y));
                        Drops[j].SetPosition(new Vector2(pos2.X + 1, pos2.Y));
                    }
                }
            }

            CheckIfLoadingZoneTriggered();
        }
        else if (State == MapManagerState.TransitioningOut)
        {
            CalculateCameraPosition();

            QueuedMapTransition.ScreenFadeTimer.Update(gameTime);
            QueuedMapTransition.MoveOutLerper.Update(gameTime);

            Globals.Player.SetPosition(QueuedMapTransition.MoveOutLerper.GetLerpedValue());

            if (QueuedMapTransition.ScreenFadeTimer.IsDone() && QueuedMapTransition.MoveOutLerper.IsDone())
            {
                SwitchMap();
                SetState(MapManagerState.TransitioningIn);
            }
        }
        else if (State == MapManagerState.TransitioningIn)
        {
            CalculateCameraPosition();

            QueuedMapTransition.MoveInLerper.Update(gameTime);
            TransitionInLerper.Update(gameTime);

            Globals.Player.SetPosition(QueuedMapTransition.MoveInLerper.GetLerpedValue());

            if (TransitionInLerper.IsDone() && QueuedMapTransition.MoveInLerper.IsDone())
            {
                SetState(MapManagerState.Normal);
            }
        }
    }

    public void AddDrop(IDropInstance drop)
    {
        Drops.Add(drop);
        DropHoverTimers.Add(0);
        DropInitialPositions.Add(drop.GetPosition());
    }

    public bool IsMapTransitioning() => QueuedMapTransition != null;

    #endregion

    #region Private Methods

    private void DrawTileLayer(SpriteBatch spriteBatch, TileLayer tileLayer)
    {
        foreach (var tile in tileLayer.Tiles)
        {
            spriteBatch.Draw(texture: tile.TileSet.SpriteSheetTexture,
                destinationRectangle: new Rectangle(
                    new Point(tile.X * tile.TileSet.TileWidth, tile.Y * tile.TileSet.TileHeight),
                    new Point(tile.TileSet.TileWidth, tile.TileSet.TileHeight)),
                sourceRectangle: tile.SourceRectangle,
                color: Color.White);
        }
    }

    private void DrawPlayer(PlayerInstance player)
    {
        var flip = player.GetFacingDirection() == Direction.Left;
        GameManager.SpriteBatch.Draw(player.GetSpriteSheet(),
            new Rectangle(Vector2.Round(player.GetPosition()).ToPoint(), player.GetSize()),
            player.GetTextureRectangle(),
            Color.White,
            0F,
            Vector2.Zero,
            flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
            0);
    }

    private void UpdateDrops()
    {
        for (var i = 0; i < Drops.Count; i++)
        {
            DropHoverTimers[i]++;
            var iPos = DropInitialPositions[i];
            var pos = Drops[i].GetPosition();
            Drops[i].SetPosition(new Vector2(pos.X, iPos.Y + 10 * (float)Math.Sin((double)DropHoverTimers[i] / 50)));
        }
    }

    private void CalculateCameraPosition()
    {
        var player = Globals.Player;
        var camera = new OrthographicCamera(GameManager.SpriteBatch.GraphicsDevice)
        {
            Zoom = 3F
        };

        var x = camera.BoundingRectangle;
        var mapWidth = CurrentMap.Width * CurrentMap.TileWidth;
        var mapHeight = CurrentMap.Height * CurrentMap.TileHeight;

        var cameraPos = Vector2.Zero;
        // Center the player
        cameraPos.X = player.GetPosition().X + player.GetSize().X / 2F;
        cameraPos.Y = player.GetPosition().Y + player.GetSize().Y / 2F;

        if (cameraPos.X - x.Width / 2 < 0)
        {
            cameraPos.X = x.Width / 2;
        }
        else if (cameraPos.X + x.Width / 2 > mapWidth)
        {
            cameraPos.X = mapWidth - x.Width / 2;
        }

        if (cameraPos.Y - x.Height / 2F < 0)
        {
            cameraPos.Y = x.Height / 2F;
        }
        else if (cameraPos.Y + x.Height / 2F > mapHeight)
        {
            cameraPos.Y = mapHeight - x.Height / 2F;
        }

        camera.LookAt(Vector2.Round(cameraPos));
        Globals.Camera = camera;
    }

    private bool IsPlayerAboveDropInstance(out List<IDropInstance> drops)
    {
        drops = new List<IDropInstance>();

        var playerBounds = Globals.Player.GetBounds();

        foreach (var dropInstance in Drops)
        {
            var pos = dropInstance.GetPosition();
            var dropBounds = new Rectangle((int)pos.X, (int)pos.Y, 16, 16);
            if (dropBounds.Intersects(playerBounds))
            {
                drops.Add(dropInstance);
            }
        }

        return drops.Count > 0;
    }

    private void CheckIfLoadingZoneTriggered()
    {
        var playerCollider = Globals.Player.GetCollisionBox();

        if (IgnoreLoadingZone)
        {
            if (CurrentMap.LoadingZones.Any(loadingZone => playerCollider.Intersects(loadingZone.GetCollider())))
            {
                return;
            }

            IgnoreLoadingZone = false;
        }
        else
        {
            foreach (var loadingZone in CurrentMap.LoadingZones)
            {
                var loadingZoneCollider = loadingZone.GetCollider();

                if (playerCollider.Intersects(loadingZoneCollider))
                {
                    QueueMapTransition(loadingZone);
                }
            }
        }
    }

    private void SetState(MapManagerState state)
    {
        State = state;
        switch (state)
        {
            case MapManagerState.Normal:
                QueuedMapTransition = null;
                break;
            case MapManagerState.TransitioningOut:
                IgnoreLoadingZone = true;
                break;
            case MapManagerState.TransitioningIn:
                TransitionInLerper = new LinearFloatLerper(1F, 0F, 1.0);
                break;
        }
    }

    private void QueueMapTransition(LoadingZone loadingZone)
    {
        SetState(MapManagerState.TransitioningOut);
        var player = Globals.Player;

        var moveOutDestination = loadingZone.MoveOut switch
        {
            Direction.Up => new Vector2(0, -32),
            Direction.Down => new Vector2(0, 32),
            Direction.Left => new Vector2(-32, 0),
            Direction.Right => new Vector2(32, 0),
            _ => Vector2.Zero
        };

        var moveInSource = loadingZone.MoveIn switch
        {
            Direction.Up => new Vector2(0, 32),
            Direction.Down => new Vector2(0, -32),
            Direction.Left => new Vector2(32, 0),
            Direction.Right => new Vector2(-32, 0),
            _ => Vector2.Zero
        };

        QueuedMapTransition = new MapTransition
        {
            Map = Maps[loadingZone.ToMap],
            ToPosition = loadingZone.ToPosition,
            ScreenFadeTimer = new LinearTimer(1),
            MoveOut = loadingZone.MoveOut,
            MoveIn = loadingZone.MoveIn,
            MoveOutLerper = new LinearVector2Lerper(player.GetPosition(), player.GetPosition() + moveOutDestination, 1),
            MoveInLerper = new LinearVector2Lerper(loadingZone.ToPosition + moveInSource, loadingZone.ToPosition, 1)
        };
    }

    private void SwitchMap()
    {
        CurrentMap = QueuedMapTransition.Map;
        Globals.Player.SetPosition(QueuedMapTransition.ToPosition);
    }

    #endregion

    private enum MapManagerState
    {
        Normal,
        TransitioningOut,
        TransitioningIn
    }

    private class MapTransition
    {
        public GameMap Map { get; set; }
        public Vector2 ToPosition { get; set; }
        public int MoveOut { get; set; }
        public int MoveIn { get; set; }

        public ITimer ScreenFadeTimer { get; set; }
        public ILerper<Vector2> MoveOutLerper { get; set; }
        public ILerper<Vector2> MoveInLerper { get; set; }
    }
}