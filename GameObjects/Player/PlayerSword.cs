using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace SlashySam.GameObjects.Player
{
    public class PlayerSword : GameObject
    {
        // Player reference
        Player player;

        public PlayerSword(Player _player)
        {
            player = _player;
            Texture = Game.Assets.Player.PlayerKatana01;
            Scale = new Vector2(2f);
        }

        bool Spiking
        {
            get;
            set;
        } = false;

        public bool Attacking { get; set; } = false;

        float radius = 60;

        protected override void UpdateMyself()
        {
            var mouseInWorld = Helper.MouseInWorld;

            Vector2 direction = Vector2.Normalize(new Vector2(mouseInWorld.X - player.Position.X, mouseInWorld.Y - player.Position.Y));

            float a = MathF.Atan2(direction.Y, direction.X);

            Rotation = a + MathF.PI / 2;

            var mouse = Mouse.GetState();

            if (mouse.LeftButton == ButtonState.Pressed && GameScreen.ActualScreenRectangle.Includes(mouse.Position) &&radius <= 60 && player.Energy > player.swordAttackCost)
            {
                player.ApplySwordHitCost();
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

            int x = (int)(player.Position.X + MathF.Cos(a) * radius);
            int y = (int)(player.Position.Y + MathF.Sin(a) * radius);
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
