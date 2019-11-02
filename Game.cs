using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using System.Collections.Generic;

using Comora;

namespace SlashySam
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        public static GraphicsDeviceManager Graphics
        {
            get;
            private set;
        }

        public static SpriteBatch SpriteBatch
        {
            get;
            private set;
        }

        public static float DeltaTime
        {
            get;
            private set;
        }

        public static GameTime GameTime
        {
            get;
            private set;
        }

        public static Point Screen
        {
            get
            {
                return new Point(1280, 720);
            }
        }

        public static Camera Camera
        {
            get;
            private set;
        }

        public static Rectangle CameraBounds
        {
            get
            {
                return Helper.RectangleFromPosition(Camera.Position, (int)(Screen.X * Camera.Transform.Scale.X), (int)(Screen.Y * Camera.Transform.Scale.Y));
            }
        }

        public static bool ExitButtonClicked { get; set; } = false;

        public Game()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
            Graphics.PreferredBackBufferWidth = Screen.X;
            Graphics.PreferredBackBufferHeight = Screen.Y;

            // V Sync
            Graphics.SynchronizeWithVerticalRetrace = false;

            // Framerate
            IsFixedTimeStep = false;
        }

        protected override void Initialize()
        {
            Camera = new Camera(GraphicsDevice);
            base.Initialize();
        }

        private static Dictionary<string, Screen> screens = new Dictionary<string, Screen>();
        private static Screen currentScreen;
        public static string CurrentScreenID => currentScreen.ID;
        public static void SetScreen(string id)
        {
            if (screens[id] != null)
            {
                if (currentScreen != null)
                {
                    currentScreen.OnExit();
                }
                currentScreen = screens[id];
                currentScreen.OnEnter();
            }
        }

        protected override void LoadContent()
        {
            Assets.ContentManager = Content;
            Helper.Init();
            Camera.LoadContent();

            screens.Add(GameScreen.Id, new GameScreen());
            screens.Add(MainMenu.Id, new MainMenu());
            screens.Add(UpgradesScreen.Id, new UpgradesScreen());

            foreach (var screen in screens.Values)
            {
                screen.Load();
            }

            SetScreen(GameScreen.Id);

            SpriteBatch = new SpriteBatch(GraphicsDevice);
        }

        Vector2 mousePosition;

        float titleUpdateCD = 0.15f;

        protected override void Update(GameTime gameTime)
        {
            GameTime = gameTime;
            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (titleUpdateCD < 0)
            {
                Window.Title = "SlashySam | FPS: " + System.MathF.Round(Camera.Debug.FpsCounter.CurrentFramesPerSecond).ToString();
                titleUpdateCD = 0.05f;
            }
            else
            {
                titleUpdateCD -= DeltaTime;
            }

            mousePosition = Mouse.GetState().Position.ToVector2();

            Camera.Debug.IsVisible = Keyboard.GetState().IsKeyDown(Keys.F1);
            Camera.Update(gameTime);

            if (currentScreen != null)
            {
                currentScreen.Update();
            }

            if (ExitButtonClicked)
            {
                Exit();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (currentScreen != null)
            {
                currentScreen.Draw();
            }

            SpriteBatch.Begin();
            var origin = new Vector2(Assets.Cursor.Width / 2);
            var scale = new Vector2(1.7f);
            SpriteBatch.Draw(Assets.Cursor, mousePosition, null, Color.Red, 0, origin, scale, SpriteEffects.None, 0);
            SpriteBatch.End();

            SpriteBatch.Draw(Camera.Debug);
            base.Draw(gameTime);
        }

        public static void Debug(object obj)
        {
            System.Diagnostics.Debug.WriteLine(obj);
        }

        public static class Assets
        {
            public static Microsoft.Xna.Framework.Content.ContentManager ContentManager
            {
                get;
                set;
            }
            private static Texture2D Load(string name) => ContentManager.Load<Texture2D>(name);

            public static Texture2D Rectangle => Load("Rectangle");
            public static Texture2D Circle => Load("Circle");
            public static Texture2D PixelatedCircle => Load("PixelatedCircle");
            public static Texture2D Cursor => Load("Cursor");
            public static Texture2D Cross => Load("Cross");

            public static Texture2D SplashArt => Load("SplashArt_02");

            public static SpriteFont ThaleahFont => ContentManager.Load<SpriteFont>("ThaleahFont");

            public static class Levels
            {
                private static Texture2D Load(string name) => ContentManager.Load<Texture2D>("Levels/" + name);

                public static Texture2D Level_01 => Load("Level_01");
            }

            public static class Player
            {
                private static Texture2D Load(string name) => ContentManager.Load<Texture2D>("Player/" + name);
                public static Texture2D PlayerKatana01 => Load("Katana-01");
                public static Texture2D Sam01 => Load("Sam-01");
                public static Texture2D Sam => Load("Sam");
                public static Texture2D Shuriken => Load("Shuriken");
                public static Texture2D SamHead => Load("SamHead");
                public static Texture2D SmokeBombIcon => Load("SmokeBombIcon");
            }

            public static class Enemies
            {
                private static Texture2D Load(string name) => ContentManager.Load<Texture2D>("Enemies/" + name);
                public static Texture2D Scout => Load("Scout");
                public static Texture2D Scout_Sword => Load("Scout_Sword");
            }

            public static class Env
            {
                private static Texture2D Load(string name) => ContentManager.Load<Texture2D>("Environment/" + name);

                public static Texture2D GrassSet => Load("GrassSet");
                public static Texture2D Ground => Load("Ground");
                public static Texture2D Bush_01 => Load("Bush_01");
                public static Texture2D Sand_Steps => Load("Sand_Steps");
                public static Texture2D Flowers_01 => Load("Flowers_01");
                public static Texture2D Flowers_02 => Load("Flowers_02");
                public static Texture2D Log_01 => Load("Log_01");
                public static Texture2D Rock_01 => Load("Rock_01");
                public static Texture2D Dead_Tree => Load("Dead_Tree");
                public static Texture2D House_01 => Load("House_01");
                public static Texture2D House_02 => Load("House_02");
                public static Texture2D Box_01 => Load("Box_01");
                public static Texture2D Campfire => Load("Campfire");
            }
        }
    }
}
