using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SlashySam.Particles;
using System;

namespace SlashySam.GameObjects.Enemies
{
    public class ScoutSword : GameObject
    {
        Enemy self;
        public ScoutSword(Enemy enemy)
        {
            self = enemy;
            Texture = Game.Assets.Enemies.Scout_Sword;
            Scale = new Vector2(2.5f, 3f);
        }

        bool Spiking
        {
            get;
            set;
        } = false;

        public bool Attacking { get; set; } = false;

        float radius = 60;

        int damage = 10;

        protected override void UpdateMyself()
        {
            var playerPos = GameScreen.Player.Position;
            Vector2 direction = Vector2.Normalize(new Vector2(playerPos.X - self.Position.X, playerPos.Y - self.Position.Y));

            float a = MathF.Atan2(direction.Y, direction.X);

            Rotation = a + MathF.PI / 2;

            if (Vector2.Distance(GameScreen.Player.Position, self.Position) < radius * 2 && radius <= 60)
            {
                Spiking = true;
                Attacking = true;
            }

            if (Spiking)
            {
                radius += 300 * Game.DeltaTime;
                if (radius > 90)
                {
                    Spiking = false;
                    Attacking = false;
                }
            }

            if (radius > 60 && !Spiking)
            {
                radius -= 250 * Game.DeltaTime;
            }

            if (Attacking && Vector2.Distance(GameScreen.Player.Position, Position) < Texture.Height)
            {
                Attacking = false;
                GameScreen.Player.Health -= damage;
                Effects.Systems.Add(new ParticleSystem(GameScreen.Player.Position, Vector2.One, Effects.Blood));
            }

            int x = (int)(self.Position.X + MathF.Cos(a) * radius);
            int y = (int)(self.Position.Y + MathF.Sin(a) * radius);
            Position = new Vector2(x, y);

            if (direction.X > 0)
            {
                this.SpriteEffects = SpriteEffects.FlipHorizontally;
            }
            else if (direction.X < 0)
            {
                this.SpriteEffects = SpriteEffects.None;
            }
        }
    }
}
