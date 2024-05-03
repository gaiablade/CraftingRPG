using CraftingRPG.AssetManagement;
using CraftingRPG.Entities;
using CraftingRPG.GameStateManagement;
using CraftingRPG.GameStateManagement.GameStates;
using CraftingRPG.Global;
using CraftingRPG.InputManagement;
using CraftingRPG.MapManagement;
using CraftingRPG.SoundManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CraftingRPG
{
    public class GameManager : Game
    {
        private readonly GraphicsDeviceManager Graphics;
        public static SpriteBatch SpriteBatch { get; private set; }
        private static KeyboardState @KeyboardState { get; set; }
        public static Texture2D Pixel { get; private set; }
        public static Point Resolution { get; private set; }
        public static Rectangle WindowBounds { get; private set; }
        public static Point ScreenCenter => Vector2.Divide(Resolution.ToVector2(), 2).ToPoint();
        // public static Dictionary<ItemId, IItem> ItemInfo { get; private set; }
        public static Point PlayerSpriteSize = new Point(48, 48);

        public static GameStateManager StateManager { get; private set; } = GameStateManager.Instance;

        public GameManager()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Resolution = new Point(800, 608);
            Graphics.PreferredBackBufferWidth = Resolution.X;
            Graphics.PreferredBackBufferHeight = Resolution.Y;
            Graphics.ApplyChanges();
            Resolution = new Point(GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight);
            WindowBounds = new Rectangle(Point.Zero, Resolution);

            Pixel = new Texture2D(GraphicsDevice, 1, 1);
            Pixel.SetData(new[] { Color.White });

            // ItemInfo = new Dictionary<ItemId, IItem>
            // {
            //     { ItemId.EmptyBottle, new EmptyBottleItem() },
            //     { ItemId.HealingMushroom, new HealingMushroomItem() },
            //     { ItemId.HeartyFlower, new HeartyFlowerItem() },
            //     { ItemId.SmallHealthPotion, new SmallHealthPotionItem() },
            //     { ItemId.MediumHealthPotion, new MediumHealthPotionItem() },
            //     { ItemId.IronSword, new IronSwordItem() },
            //     { ItemId.IronChunk, new IronChunkItem() },
            //     { ItemId.IronHelmet, new IronHelmetItem() },
            //     { ItemId.IronBand, new IronBandItem() },
            //     { ItemId.ArcaneFlower, new ArcaneFlowerItem() },
            //     { ItemId.MageBracelet, new MageBraceletItem() },
            // };

            Globals.PlayerInfo = new PlayerInfo();
            Globals.Player = new PlayerInstance(Globals.PlayerInfo);
            
            InputManager.Instance.LoadKeybindingConfiguration();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            MapManager.Instance.LoadMapsFromContents(this.Content);
            Assets.Instance.LoadAssets(this.Content);

            StateManager.PushState<OverWorldGameState>(true);
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
            StateManager.CurrentGameState.Update(gameTime);

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