using CraftingRPG.Entities;
using CraftingRPG.Enums;
using CraftingRPG.Interfaces;
using CraftingRPG.Items;
using CraftingRPG.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace CraftingRPG
{
    public class GameManager : Game
    {
        private GraphicsDeviceManager _graphics;
        public static SpriteBatch SpriteBatch { get; private set; }
        public static KeyboardState @KeyboardState { get; private set; }
        public static Dictionary<Keys, int> FramesKeysHeld { get; private set; }
        public static SpriteFont DefaultFont { get; private set; }
        public static SpriteFont Fnt10 { get; private set; }
        public static SpriteFont Fnt12 { get; private set; }
        public static SpriteFont Fnt15 { get; private set; }
        public static SpriteFont Fnt20 { get; private set; }
        public static Texture2D SpriteSheet { get; private set; }
        public static Texture2D TileSet { get; private set; }
        public static Texture2D Pixel { get; private set; }
        public static Point Resolution { get; private set; }
        public static PlayerInfo PlayerInfo { get; private set; }
        public static Dictionary<ItemId, IItem> ItemInfo { get; private set; }

        public static StateManager StateManager { get; private set; } = StateManager.Instance;

        public GameManager()
        {
            // Set framerate to 60fps
            this.IsFixedTimeStep = true;
            this.TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d);

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
            };

            PlayerInfo = new PlayerInfo();
            FramesKeysHeld = new Dictionary<Keys, int>();
            StateManager.PushState<MainMenuState>(true);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            DefaultFont = Content.Load<SpriteFont>("fonts/heygorgeous");
            Fnt10 = Content.Load<SpriteFont>("fonts/rainyhearts-10px");
            Fnt12 = Content.Load<SpriteFont>("fonts/rainyhearts-12px");
            Fnt15 = Content.Load<SpriteFont>("fonts/rainyhearts-15px");
            Fnt20 = Content.Load<SpriteFont>("fonts/heygorgeous-20px");
            SpriteSheet = Content.Load<Texture2D>("textures/crpg_spritesheet");
            TileSet = Content.Load<Texture2D>("textures/crpg_tileset");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState = Keyboard.GetState();
            foreach (var (key, frames) in FramesKeysHeld)
            {
                if (KeyboardState.IsKeyDown(key))
                {
                    FramesKeysHeld[key] = frames + 1;
                }
                else
                {
                    FramesKeysHeld[key] = 0;
                }
            }

            StateManager.ProcessStateRequests();
            StateManager.CurrentState.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            SpriteBatch = new SpriteBatch(GraphicsDevice);
            SpriteBatch.Begin();

            StateManager.CurrentState.Render();

            SpriteBatch.End();

            base.Draw(gameTime);
        }

        public static void AddKeyIfNotExists(Keys key)
        {
            if (!FramesKeysHeld.ContainsKey(key))
            {
                FramesKeysHeld.Add(key, 0);
            }
        }

        public static void AddKeysIfNotExists(params Keys[] keys)
        {
            foreach (var key in keys)
            {
                if (!FramesKeysHeld.ContainsKey(key))
                {
                    FramesKeysHeld.Add(key, 0);
                }
            }
        }
    }
}