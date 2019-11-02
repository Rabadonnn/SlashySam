using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SlashySam.UI;
using SlashySam.Particles;

namespace SlashySam
{
    public class MainMenu : Screen
    {
        public static string Id => "menu";
        public override string ID => Id;

        Button playButton;
        Button quitButton;
        Button upgradeScreenButton;

        ParticleSystem RainEffect1;
        ParticleSystem RainEffect2;

        int LeftBorder => Game.Screen.Y / 10;

        public override void Load()
        {
            playButton = new Button(new Vector2(LeftBorder, Game.Screen.Y / 2 * 1.3f), 0.6f, "PLAY", () => Game.SetScreen(GameScreen.Id)) { alignLeft = true };
            upgradeScreenButton = new Button(new Vector2(LeftBorder, Game.Screen.Y / 2 * 1.5f), 0.6f, "UPGRADES", () => Game.SetScreen(UpgradesScreen.Id)) { alignLeft = true };
            quitButton = new Button(new Vector2(LeftBorder, Game.Screen.Y / 2 * 1.7f), 0.6f, "QUIT", () => Game.ExitButtonClicked = true) { alignLeft = true };

            playButton.onHoverAction = playButton.OnHoverAction;
            upgradeScreenButton.onHoverAction = upgradeScreenButton.OnHoverAction;
            quitButton.onHoverAction = quitButton.OnHoverAction;

            RainEffect1 = new ParticleSystem(new Vector2(Game.Screen.X, Game.Screen.Y / 4), new Vector2(10, Game.Screen.Y / 2), new ParticleSystemSettings()
            {
                AccX = new Size(-100, -10),
                AccY = new Size(-30, 100),
                Speed = new Size(65, 150),
                Lifetime = new Size(10, 25),
                Density = 0.2f,
                Size = new Size(0.025f, 1f),
                Texture = Helper.SquareTexture,
                Colors = new System.Collections.Generic.List<Color>()
                {
                    new Color(118, 66, 138),
                    new Color(59, 11, 78),
                    new Color(34, 0, 47)
                },
                OneTime = false,
                DecreaseAlpha = false,
                DecreaseSize = false
            });
            RainEffect2 = new ParticleSystem(new Vector2(0, Game.Screen.Y / 4), new Vector2(10, Game.Screen.Y / 2), new ParticleSystemSettings()
            {
                AccX = new Size(100, 10),
                AccY = new Size(-30, 100),
                Speed = new Size(65, 150),
                Lifetime = new Size(10, 25),
                Density = 0.2f,
                Size = new Size(0.025f, 1f),
                Texture = Helper.SquareTexture,
                Colors = new System.Collections.Generic.List<Color>()
                {
                    new Color(118, 66, 138),
                    new Color(59, 11, 78),
                    new Color(34, 0, 47)
                },
                OneTime = false,
                DecreaseAlpha = false,
                DecreaseSize = false
            });
        }

        Rectangle screenRect = new Rectangle(0, 0, Game.Screen.X, Game.Screen.Y);
        public override void Update()
        {
            playButton.Update();
            upgradeScreenButton.Update();
            quitButton.Update();

            RainEffect1.Update();
            RainEffect2.Update();

            foreach (var p in RainEffect1.Particles)
            {
                if (!screenRect.Includes(p.Position))
                {
                    p.Dead = true;
                }
            }
            foreach (var p in RainEffect2.Particles)
            {
                if (!screenRect.Includes(p.Position))
                {
                    p.Dead = true;
                }
            }
        }

        Color textColor = Color.White;
        public override void Draw()
        {
            Game.Graphics.GraphicsDevice.Clear(Color.Black);
            Game.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

            Game.SpriteBatch.Draw(Game.Assets.SplashArt, new Rectangle(0, 0, Game.Screen.X, Game.Screen.Y), Color.White);

            //RainEffect1.Draw();
            RainEffect2.Draw();

            playButton.Draw();
            upgradeScreenButton.Draw();
            quitButton.Draw();

            Game.SpriteBatch.End();
        }
    }
}
