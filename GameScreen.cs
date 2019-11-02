using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using Comora;

using SlashySam.GameObjects.Player;
using SlashySam.Levels;
using SlashySam.UI;

namespace SlashySam
{
    public class GameScreen : Screen
    {
        public static string Id => "game";
        public override string ID => Id;

        public bool Paused { get; set; } = false;

        private static Level[] levels;

        Button pauseButton;

        public static Level CurrentLevel
        {
            get;
            private set;
        }

        public static Player Player
        {
            get;
            private set;
        }
        public static int BlackBarHeight => (int)(Game.Screen.Y / 8f);

        public static Rectangle ActualScreenRectangle => new Rectangle(0, BlackBarHeight, Game.Screen.X, Game.Screen.Y - BlackBarHeight / 2);

        PopUpMenu pausePopUp;
        PopUpMenu restartPopUp;

        public override void Load()
        {

            levels = new Level[]
            {
                new LevelEditor(),
                new Level_01()
            };

            CurrentLevel = levels[0];
            Player = new Player();
            Player.Spawn(CurrentLevel.SpawnPoint);

            pauseButton = new Button(new Vector2(Game.Screen.X - 100, BlackBarHeight / 2), 0.3f, "PAUSE", () => Paused = true);

            pausePopUp = new PopUpMenu(Helper.RectangleFromPosition(new Vector2(Game.Screen.X / 2, Game.Screen.Y / 2), Game.Screen.X / 5, Game.Screen.Y / 6))
            {
                Color = Color.Black * 0.7f
            };
            restartPopUp = new PopUpMenu(Helper.RectangleFromPosition(new Vector2(Game.Screen.X / 2, Game.Screen.Y / 2), Game.Screen.X / 5, Game.Screen.Y / 6))
            {
                Color = Color.Black * 0.7f
            };

            var resumeButton = new Button(new Vector2(Game.Screen.X / 2, Game.Screen.Y / 2 - 20), 0.3f, "RESUME", () => Paused = false);
            var menuButton = new Button(new Vector2(Game.Screen.X / 2, Game.Screen.Y / 2 + 20), 0.3f, "MENU", MenuButtonPressed);

            var restartButton = new Button(new Vector2(Game.Screen.X / 2, Game.Screen.Y / 2 - 20), 0.3f, "RESTART", RestartButtonPressed);

            pausePopUp.Components.Add(resumeButton);
            pausePopUp.Components.Add(menuButton);

            restartPopUp.Components.Add(restartButton);
            restartPopUp.Components.Add(menuButton);

            Game.Camera.Zoom = 1.1f;
        }
        
        void RestartButtonPressed()
        {
            Player.Spawn(CurrentLevel.SpawnPoint);
            CurrentLevel.Restart();
        }

        void MenuButtonPressed()
        {
            Paused = false;
            Game.SetScreen(MainMenu.Id);
        }

        public override void Update()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.F3))
            {
                Game.Camera.Zoom += 0.5f * Game.DeltaTime;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.F4))
            {
                Game.Camera.Zoom -= 0.5f * Game.DeltaTime;
            }

            pauseButton.Update();
            if (!Paused)
            {
                if (!Player.Dead)
                {
                    CurrentLevel.Update();
                    Player.Update();
                }
            }
            else
            {
                pausePopUp.Update();
            }

            if (Player.Dead)
            {
                restartPopUp.Update();
            }

            Particles.Effects.Update();
        }

        public static Color BGColor => new Color(63, 58, 65);
        Rectangle topRect = new Rectangle(0, 0, Game.Screen.X, BlackBarHeight);
        public override void Draw()
        {
            Game.Graphics.GraphicsDevice.Clear(BGColor);

            if (!Player.Dead)
            {
                Game.SpriteBatch.Begin(Game.Camera, samplerState: SamplerState.PointClamp);
                CurrentLevel.Draw();
                Player.Draw();
                Particles.Effects.Draw();
                Game.SpriteBatch.End();
            }

            Game.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
            Game.SpriteBatch.Draw(Helper.SquareTexture, topRect, Color.Black * 0.5f);
            Player.DrawHUD();

            if (!Paused)
            {
                pauseButton.Draw();
            }

            if (Paused)
            {
                pausePopUp.Draw();
            }

            if (Player.Dead)
            {
                Game.SpriteBatch.Draw(Helper.SquareTexture, new Rectangle(0, 0, Game.Screen.X, Game.Screen.Y), Color.Black * 0.5f);
                restartPopUp.Draw();
            };

            Game.SpriteBatch.End();
        }
    }
}
