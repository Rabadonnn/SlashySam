using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SlashySam.UI;

namespace SlashySam
{
    public class UpgradesScreen : Screen
    {
        public static string Id => "upgrades";
        public override string ID => Id;

        Button backButton;
        public override void Load()
        {
            backButton = new Button(new Vector2(Game.Screen.Y / 10, Game.Screen.Y / 2 * 1.7f), 0.6f, "BACK", () => Game.SetScreen(MainMenu.Id)) { alignLeft = true };
        }

        public override void Update()
        {
            backButton.Update();
        }

        public override void Draw()
        {
            Game.Graphics.GraphicsDevice.Clear(Color.Transparent);
            Game.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
            Game.SpriteBatch.Draw(Game.Assets.SplashArt, new Rectangle(0, 0, Game.Screen.X, Game.Screen.Y), Color.DarkGray);
            backButton.Draw();
            Game.SpriteBatch.End();
        }
    }
}
