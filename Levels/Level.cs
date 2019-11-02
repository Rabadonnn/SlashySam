using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SlashySam.GameObjects.Enemies;
using SlashySam.GameObjects.Env;

namespace SlashySam.Levels
{
    public abstract class Level
    {
        public abstract int ID { get; }
        // Each level contains some environment
        public List<GameObject> Objects { get; } = new List<GameObject>();
        public List<Enemy> Enemies { get; } = new List<Enemy>();
        public Map Map { get; protected set; }

        public virtual Vector2 SpawnPoint => Vector2.Zero;
        public virtual Vector2 EndPoint => Vector2.Zero;

        public Level()
        {
            Restart();
        }

        public abstract void Restart();
        public virtual void Update()
        {
            UpdateObjects();
        }
        public virtual void Draw()
        {
            DrawObjects();
            if (Map != null)
            {
                Map.Draw();
            }
        }

        protected void UpdateObjects()
        {
            UpdateEnemies();
            foreach (var obj in Objects.ToArray())
            {
                if (obj is Breakable || obj is Campfire)
                {
                    if (obj.Dead) Objects.Remove(obj);
                    else obj.Update();
                }
            }
        }

        protected void UpdateEnemies()
        {
            foreach (var enemy in Enemies.ToArray())
            {
                if (enemy.Dead)
                {
                    Enemies.Remove(enemy);
                }
                else
                {
                    enemy.Update();

                    foreach (var s in GameScreen.Player.Shurikens)
                    {
                        if (s.Rectangle.Intersects(enemy.Rectangle))
                        {
                            s.Dead = true;
                            enemy.Health -= GameScreen.Player.ShurikenDamage;
                            Particles.Effects.Systems.Add(new Particles.ParticleSystem(enemy.Position, new Vector2(1), Particles.Effects.Blood));
                        }
                    }

                    if (GameScreen.Player.Sword.Attacking && Vector2.Distance(GameScreen.Player.Sword.Position, enemy.Position) < GameScreen.Player.Sword.Rectangle.Height / 2 + enemy.Rectangle.Width / 2)
                    {
                        GameScreen.Player.Sword.Attacking = false;
                        enemy.Health -= GameScreen.Player.SwordDamage;
                        Particles.Effects.Systems.Add(new Particles.ParticleSystem(enemy.Position, new Vector2(1), Particles.Effects.Blood));
                    }
                }
            }
        }

        protected void DrawObjects()
        {
            foreach (var obs in Objects)
            {
                if (Game.CameraBounds.Includes(obs.Position))
                {
                    obs.Draw();
                }
            }
            foreach (var e in Enemies)
            {
                if (Game.CameraBounds.Includes(e.Position))
                {
                    e.Draw();
                }
            }
        }
    }
}
