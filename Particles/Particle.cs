using Microsoft.Xna.Framework;
using System;

namespace SlashySam.Particles
{
    public class Particle : GameObject
    {
        float size;
        float initial_size;
        int speed;
        float lifetime;
        float initial_lifetime;
        Vector2 acceleration;
        float alpha = 1;
        ParticleSystemSettings Settings { get; }
        public Particle(ParticleSystemSettings settings, Vector2 position)
        {
            Position = position;
            size = settings.Size.RandomFloat();
            initial_size = size;
            speed = settings.Speed.RandomInt();
            lifetime = settings.Lifetime.RandomFloat();
            initial_lifetime = lifetime;
            acceleration = new Vector2(settings.AccX.RandomInt(), settings.AccY.RandomInt());
            if (settings.Textures.Count == 0)
            {
                Texture = settings.Texture;
            }
            else
            {
                Texture = settings.Textures[Helper.Rand.Next(settings.Textures.Count)];
            }

            if (settings.Colors.Count == 0)
            {
                Color = settings.Color;
            }
            else
            {
                Color = settings.Colors[Helper.Rand.Next(settings.Colors.Count)];
            }
            Settings = settings;
        }

        protected override void UpdateMyself()
        {
            if (!Dead)
            {
                if (speed != 0)
                {
                    acceleration.Normalize();
                    acceleration *= speed * Game.DeltaTime;
                    Position += acceleration;
                }

                lifetime -= Game.DeltaTime;

                acceleration.Y += Settings.Gravity;

                if (Settings.DecreaseSize)
                {
                    size = Helper.MapValue(lifetime, initial_lifetime, 0, initial_size, 0);
                }
                else if (Settings.IncreaseSize)
                {
                    size = Helper.MapValue(lifetime, initial_lifetime, 0, initial_size, initial_size * Settings.MaxSizeFromInitialSize);
                }

                if (Settings.DecreaseAlpha)
                {
                    alpha = Helper.MapValue(lifetime, initial_lifetime, 0, Settings.StartingAlpha, 0);
                }

                if (lifetime < 0)
                {
                    Dead = true;
                }
                Scale = new Vector2(size);
            }
        }

        protected override void DrawMyself()
        {
            Game.SpriteBatch.Draw(Texture, Position, sourceRectangle, Color * alpha, Rotation, Origin, Scale, this.SpriteEffects, 0);
        }
    }
}
