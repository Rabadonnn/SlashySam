using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SlashySam.Particles
{
    public class ParticleSystem
    {
        public bool Enabled { get; set; } = true;
        public bool Dead { get; set; } = false;
        public ParticleSystemSettings Settings { get; private set; }
        public Vector2 Position { get; set; }
        public Vector2 Scale { get; set; } = Vector2.Zero;
        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)(Position.X - Scale.X / 2), (int)(Position.Y - Scale.Y / 2), (int)Scale.X, (int)Scale.Y);
            }
        }

        public List<Particle> Particles { get; private set; }
        float lifetime;
        public bool Finished { get; private set; } = false;
        bool shouldAdd = true;
        bool decimalDensity = false;
        float currentDensity = 0;
        public ParticleSystem(Vector2 position, Vector2 scale, ParticleSystemSettings settings)
        {
            Position = position;
            Scale = scale;
            Settings = settings;
            Particles = new List<Particle>();
            lifetime = Settings.SystemLifetime;
            decimalDensity = settings.Density < 1;
        }

        public ParticleSystem(Vector2 position, ParticleSystemSettings settings)
        {
            Position = position;
            Settings = settings;
            Particles = new List<Particle>();
            lifetime = Settings.SystemLifetime;
            decimalDensity = settings.Density < 1;
        }

        Particle DefaultParticle => new Particle(new ParticleSystemSettings() { Size = new Size(0, 0), Lifetime = new Size(0, 0) }, Vector2.Zero);
        public void Update()
        {
            if (!Dead)
            {
                if (Finished)
                {
                    Dead = true;
                }
                else
                {
                    if (shouldAdd && Enabled)
                    {
                        Particle particle = DefaultParticle;
                        if (Settings.Burst)
                        {
                            for (int i = 0; i < Settings.BurstSize; i++)
                            {
                                if (!Settings.SpawnInCircle)
                                {
                                    particle = new Particle(Settings, Rectangle.RandomVectorInside());
                                }
                                else
                                {
                                    particle = new Particle(Settings, Helper.RandomPointOnCircle(Position, Helper.Rand.Next(Settings.CircleRadius)));
                                }
                            }
                            shouldAdd = false;
                        }
                        else if (decimalDensity)
                        {

                            if (currentDensity >= 1)
                            {
                                if (Scale == Vector2.Zero)
                                {
                                    particle = new Particle(Settings, Position);
                                }
                                else
                                {
                                    if (!Settings.SpawnInCircle)
                                    {
                                        particle = new Particle(Settings, Rectangle.RandomVectorInside());
                                    }
                                    else
                                    {
                                        particle = new Particle(Settings, Helper.RandomPointOnCircle(Position, Helper.Rand.Next(Settings.CircleRadius)));
                                    }
                                }
                                currentDensity = 0;
                            }
                            else
                            {
                                currentDensity += Game.DeltaTime * Settings.DensityIncreaser;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < Settings.Density; i++)
                            {
                                if (Scale == Vector2.Zero)
                                {
                                    particle = new Particle(Settings, Position);
                                }
                                else
                                {
                                    if (!Settings.SpawnInCircle)
                                    {
                                        particle = new Particle(Settings, Rectangle.RandomVectorInside());
                                    }
                                    else
                                    {
                                        particle = new Particle(Settings, Helper.RandomPointOnCircle(Position, Helper.Rand.Next(Settings.CircleRadius)));
                                    }
                                }
                            }
                        }

                        if (particle != DefaultParticle)
                        {
                            if (!Settings.SpawnParticleAtTheEnd) Particles.Insert(0, particle);
                            else Particles.Add(particle);
                        }

                        if (Settings.OneTime)
                        {
                            lifetime -= Game.DeltaTime;
                            if (lifetime < 0)
                            {
                                shouldAdd = false;
                            }
                        }
                    }

                    if (Particles.Count == 0 && !shouldAdd)
                    {
                        Finished = true;
                    }

                    if (Settings.MaxParticles != 0 && Particles.Count > Settings.MaxParticles)
                    {
                        int count = Particles.Count - Settings.MaxParticles - 1;
                        if (!Settings.SpawnParticleAtTheEnd)
                            Particles.RemoveRange(Settings.MaxParticles + 1, count);
                        else 
                            Particles.RemoveRange(0, count);
                    }

                    foreach (var p in Particles.ToArray())
                    {
                        if (p.Dead)
                        {
                            Particles.Remove(p);
                        }
                        else
                        {
                            p.Update();
                        }
                    }
                }
            }
        }

        public void Draw()
        {
            foreach (var p in Particles)
            {
                p.Draw();
            }
        }
    }
}
