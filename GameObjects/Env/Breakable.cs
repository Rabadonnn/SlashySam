using Microsoft.Xna.Framework;
using SlashySam.Particles;
using SlashySam.UI;
using System.Collections.Generic;

namespace SlashySam.GameObjects.Env
{
    public class Breakable : GameObject
    {
        Vector2 scaleOnHit;
        Vector2 initialScale;
        int health = 4;
        bool destroyed = false;
        ParticleSystem destroyEffect;
        PopUp popUp;
        int popUpSize = 32;

        public Breakable(Vector2 position)
        {
            Position = position;
            Texture = Game.Assets.Env.Box_01;

            int rand = Helper.Rand.Next(3);
            if (rand == 0)
            {
                Scale = Texture.CreateScale(42);
                scaleOnHit = Texture.CreateScale(58);
            }
            else if (rand == 1)
            {
                Scale = Texture.CreateScale(42, 32);
                scaleOnHit = Texture.CreateScale(58, 44);
            }
            else if (rand == 2)
            {
                Scale = Texture.CreateScale(40, 50);
                scaleOnHit = Texture.CreateScale(50, 60);
            }
            initialScale = Scale;
            destroyEffect = new ParticleSystem(Position, Vector2.One, new ParticleSystemSettings()
            {
                Density = 0.3f,
                Lifetime = new Size(0.4f, 0.8f),
                Size = new Size(1f, 1.5f),
                Speed = new Size(100, 200),
                Texture = Game.Assets.PixelatedCircle,
                OneTime = true,
                DecreaseSize = true,
                DecreaseAlpha = true,
                Colors = new List<Color>()
                {
                    new Color(185, 127, 86),
                    new Color(124, 78, 46),
                    new Color(153, 100, 62)
                },
                SpawnInCircle = true,
                CircleRadius = Rectangle.Width / 2,
                Burst = true,
                BurstSize = 10,
            });
            popUp = new PopUp(Position, Vector2.One, Helper.SquareTexture, 0.75f);
            Color = Helper.Rand.Next(2) == 1 ? Color.Black : Color.White;
            HasCollider = true;
        }

        protected override void UpdateMyself()
        {
            if (!destroyed)
            {
                if (GameScreen.Player.Sword.Attacking && Vector2.Distance(GameScreen.Player.Sword.Position, Position) < GameScreen.Player.Sword.Rectangle.Height / 2 + Rectangle.Width / 2)
                {
                    GameScreen.Player.Sword.Attacking = false;
                    health--;
                    Scale = scaleOnHit;
                    Color = Color.MediumVioletRed;
                }
                if (Scale.X > initialScale.X)
                {
                    Scale -= new Vector2(initialScale.X * 3 * Game.DeltaTime);
                }
                else
                {
                    Color = Color.White;
                }
                if (health == 0)
                {
                    destroyed = true;
                    int rand = Helper.Rand.Next(3);
                    if (rand == 0)
                    {
                        GameScreen.Player.CurrentShurikens += Helper.Rand.Next(2, 5);
                        popUp.Texture = Game.Assets.Player.Shuriken;
                    }
                    else if (rand == 1)
                    {
                        GameScreen.Player.CurrentSmokeBombs += Helper.Rand.Next(1, 3);
                        popUp.Texture = Game.Assets.Player.SmokeBombIcon;
                    }
                    else if (rand == 2 && GameScreen.Player.Health != GameScreen.Player.MaxHealth)
                    {
                        GameScreen.Player.Health += Helper.Rand.Next(GameScreen.Player.MaxHealth / 7, GameScreen.Player.MaxHealth / 3);
                        popUp.Texture = Game.Assets.Cross;
                        popUp.Color = Color.Green;
                    }
                    else
                    {
                        Dead = true;
                    }
                    popUp.Scale = popUp.Texture.CreateScale(popUpSize);
                }
            }
            else
            {
                destroyEffect.Update();
                popUp.Update();
                Dead = popUp.Dead && destroyEffect.Dead;
            }
        }

        protected override void DrawMyself()
        {
            if (!destroyed)
            {
                DefaultDraw();
            }
            else
            {
                destroyEffect.Draw();
                popUp.Draw();
            }
        }
    }
}
