﻿using CraftingRPG.Entities;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using CraftingRPG.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using CraftingRPG.AssetManagement;
using CraftingRPG.GameStateManagement;
using CraftingRPG.GameStateManagement.States;
using CraftingRPG.Global;
using CraftingRPG.InputManagement;
using CraftingRPG.MapManagement;
using CraftingRPG.SoundManagement;

namespace CraftingRPG
{
    public class GameManager : Game
    {
        private GraphicsDeviceManager _graphics;
        public static SpriteBatch SpriteBatch { get; private set; }
        public static KeyboardState @KeyboardState { get; private set; }
        public static Texture2D SpriteSheet { get; private set; }
        public static Texture2D TileSet { get; private set; }
        public static Texture2D Pixel { get; private set; }
        public static Point Resolution { get; private set; }
        public static Point ScreenCenter => Vector2.Divide(Resolution.ToVector2(), 2).ToPoint();
        public static PlayerInfo PlayerInfo { get; private set; }
        public static Dictionary<ItemId, IItem> ItemInfo { get; private set; }
        public static Point PlayerSpriteSize = new Point(48, 48);

        public static GameStateManager StateManager { get; private set; } = GameStateManager.Instance;

        public GameManager()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Resolution = new Point(800, 608);
            _graphics.PreferredBackBufferWidth = Resolution.X;
            _graphics.PreferredBackBufferHeight = Resolution.Y;
            _graphics.ApplyChanges();
            Resolution = new Point(GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight);

            Pixel = new Texture2D(GraphicsDevice, 1, 1);
            Pixel.SetData(new[] { Color.White });

            ItemInfo = new Dictionary<ItemId, IItem>
            {
                { ItemId.EmptyBottle, new EmptyBottleItem() },
                { ItemId.HealingMushroom, new HealingMushroomItem() },
                { ItemId.HeartyFlower, new HeartyFlowerItem() },
                { ItemId.SmallHealthPotion, new SmallHealthPotionItem() },
                { ItemId.MediumHealthPotion, new MediumHealthPotionItem() },
                { ItemId.IronSword, new IronSwordItem() },
                { ItemId.IronChunk, new IronChunkItem() },
                { ItemId.IronHelmet, new IronHelmetItem() },
                { ItemId.IronBand, new IronBandItem() },
                { ItemId.ArcaneFlower, new ArcaneFlowerItem() },
                { ItemId.MageBracelet, new MageBraceletItem() },
            };

            PlayerInfo = new PlayerInfo();
            
            InputManager.Instance.LoadKeybindingConfiguration();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            SpriteSheet = Content.Load<Texture2D>("textures/crpg_spritesheet");
            TileSet = Content.Load<Texture2D>("textures/crpg_tileset");

            MapManager.Instance.LoadMapsFromContents(this.Content);
            Assets.Instance.LoadAssets(this.Content);

            StateManager.PushState<OverworldState>(true);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Time.Delta = gameTime.ElapsedGameTime.TotalSeconds;
            
            SoundManager.Instance.Update(gameTime);

            KeyboardState = Keyboard.GetState();
            InputManager.Instance.Update(KeyboardState, gameTime);

            StateManager.ProcessStateRequests();
            StateManager.CurrentState.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            SpriteBatch = new SpriteBatch(GraphicsDevice);
            SpriteBatch.Begin(samplerState: SamplerState.PointClamp,
                transformMatrix: Globals.Camera.GetViewMatrix());
            
            foreach (var state in StateManager.States)
            {
                state.DrawWorld();
            }

            SpriteBatch.End();
            
            SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
            
            foreach (var state in StateManager.States)
            {
                state.DrawUI();
            }
            
            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}