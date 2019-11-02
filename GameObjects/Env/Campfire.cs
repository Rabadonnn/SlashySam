using SlashySam.Particles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SlashySam.GameObjects.Env
{
    public class Campfire : GameObject
    {
        ParticleSystem fire;
        ParticleSystem light;

        public Campfire(Vector2 position)
        {
            Position = position;
            Texture = Game.Assets.Env.Campfire;
            Scale = new Vector2(3f);
            HasCollider = true;

            fire = new ParticleSystem(new Vector2(position.X, position.Y - Rectangle.Height / 3), new Vector2(10, 5), new ParticleSystemSettings()
            {
                Density = 0.05f,
                DensityIncreaser = 3,
                Speed = new Size(10, 20),
                Size = new Size(0.75f, 1.3f),
                Lifetime = new Size(3f, 10),
                DecreaseAlpha = true,
                DecreaseSize = true,
                AccY = new Size(10, -100),
                Colors = new List<Color>()
                {
                    Color.Orange,
                    new Color(235, 91, 52),
                    new Color(227, 101, 48)
                },
                Texture = Game.Assets.PixelatedCircle,
                SpawnInCircle = false,
                Gravity = -0.005f,
                CircleRadius = 18,
                StartingAlpha = 0.7f
            });

            light = new ParticleSystem(position, new Vector2(10), new ParticleSystemSettings()
            {
                Density = 0.9f,
                DensityIncreaser = 0.5f,
                Speed = new Size(0, 1),
                Size = new Size(2f, 10),
                Lifetime = new Size(8f, 20),
                DecreaseAlpha = true,
                IncreaseSize = true,
                Color = Color.Yellow,
                Texture = Game.Assets.PixelatedCircle,
                StartingAlpha = 0.1f,
                MaxParticles = 3,
            });
        }

        float tick = 1;
        protected override void UpdateMyself()
        {
            fire.Update();
            light.Update();

            if (tick < 0)
            {
                if (Vector2.Distance(Position, GameScreen.Player.Position) < Rectangle.Width + Rectangle.Width / 5)
                {
                    GameScreen.Player.Health -= GameScreen.Player.MaxHealth / 10;
                }
                tick = 0.5f;
            } else tick -= Game.DeltaTime;
        }

        protected override void DrawMyself()
        {
            DefaultDraw();
            fire.Draw();
            light.Draw();
        }
    }
}
