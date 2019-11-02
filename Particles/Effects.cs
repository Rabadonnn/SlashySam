using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SlashySam.Particles
{
    public static class Effects
    {
        public static List<ParticleSystem> Systems { get; } = new List<ParticleSystem>();

        public static ParticleSystemSettings Blood
        {
            get
            {
                return new ParticleSystemSettings()
                {
                    AccY = new Size(100, -10),
                    Density = 0.2f,
                    Lifetime = new Size(0.1f, 0.3f),
                    Size = new Size(0.1f, 0.4f),
                    Speed = new Size(300, 500),
                    Textures = new List<Texture2D>()
                    {
                        Game.Assets.Rectangle,
                        Game.Assets.Circle
                    },
                    OneTime = true,
                    DecreaseSize = true,
                    DecreaseAlpha = false,
                    Colors = new List<Color>()
                    {
                        Color.Red
                    },
                    Burst = true
                };
            }
        }

        public static void Update()
        {
            foreach (var e in Systems.ToArray())
            {
                if (e.Dead)
                {
                    Systems.Remove(e);
                }
                else
                {
                    e.Update();
                }
            }
        }

        public static void Draw()
        {
            foreach (var e in Systems.ToArray())
            {
                e.Draw();
            }
        }
    }
}
