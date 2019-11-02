using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SlashySam.Particles
{
    public class ParticleSystemSettings
    {
        public float Density { get; set; } = 1;
        public Size Lifetime { get; set; } = new Size(1, 2);
        public Size Size { get; set; } = new Size(10, 20);
        public Size AccX { get; set; } = new Size(-100, 100);
        public Size AccY { get; set; } = new Size(-100, 100);
        public Size Speed { get; set; } = new Size(10, 20);
        public List<Texture2D> Textures { get; set; } = new List<Texture2D>();
        public Texture2D Texture { get; set; } = Helper.SquareTexture;
        public bool OneTime { get; set; } = false;
        public float SystemLifetime { get; set; } = 1;
        public bool DecreaseSize { get; set; } = false;
        public bool IncreaseSize { get; set; } = false;
        public float MaxSizeFromInitialSize { get; set; } = 2;
        public bool DecreaseAlpha { get; set; } = false;
        public Color Color { get; set; } = Color.White;
        public List<Color> Colors { get; set; } = new List<Color>();
        public bool SpawnInCircle { get; set; } = false;
        public int CircleRadius { get; set; } = 10;
        public bool Burst { get; set; } = false;
        public int BurstSize = 10;
        public float Gravity { get; set; } = 0;
        public bool SpawnParticleAtTheEnd { get; set; } = false;
        public float DensityIncreaser { get; set; } = 20;
        public float StartingAlpha { get; set; } = 1;
        public int MaxParticles { get; set; } = 0;
    }
}
