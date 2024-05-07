using System.Linq;
using CraftingRPG.AssetManagement;
using CraftingRPG.Entities;
using CraftingRPG.GameStateManagement;
using CraftingRPG.GameStateManagement.GameStates;
using CraftingRPG.Global;
using CraftingRPG.InputManagement;
using CraftingRPG.MapManagement;
using CraftingRPG.Player;
using CraftingRPG.SoundManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

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
            Globals.Camera = new OrthographicCamera(GraphicsDevice);

            Pixel = new Texture2D(GraphicsDevice, 1, 1);
            Pixel.SetData(new[] { Color.White });

            Globals.PlayerInfo = new PlayerInfo();
            Globals.Player = new PlayerInstance(Globals.PlayerInfo);
            
            InputManager.Instance.LoadKeybindingConfiguration();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            MapManager.Instance.LoadMapsFromContents(Content);
            Assets.Instance.LoadAssets(Content);

            StateManager.PushState<MainMenuGameState>(true);
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
            foreach (var state in StateManager.States.Reverse())
            {
                state.Update(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            SpriteBatch = new SpriteBatch(GraphicsDevice);
            SpriteBatch.Begin(samplerState: SamplerState.PointClamp,
                transformMatrix: Globals.Camera.GetViewMatrix());
            
            foreach (var state in StateManager.States.Reverse())
            {
                state.DrawWorld();
            }

            SpriteBatch.End();
            
            SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
            
            foreach (var state in StateManager.States.Reverse())
            {
                state.DrawUi();
            }
            
            SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}