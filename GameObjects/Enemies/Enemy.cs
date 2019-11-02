using Microsoft.Xna.Framework;

namespace SlashySam.GameObjects.Enemies
{
    public abstract class Enemy : GameObject
    {
        public enum EnemyType
        {
            Scout
        }

        protected int maxHealth;
        public int Health { get; set; }

        public EnemyType Type { get; set; }

        protected bool CollidingBot { get; set; }
        protected bool CollidingTop { get; set; }
        protected bool CollidingRight { get; set; }
        protected bool CollidingLeft { get; set; }

        protected void Initialize()
        {
            HasCollider = true;
            if (Type == EnemyType.Scout)
            {
                maxHealth = 100;
                Health = maxHealth;
            }
        }

        protected virtual void CheckCollision()
        {
            var objs = new System.Collections.Generic.List<GameObject>(GameScreen.CurrentLevel.Objects);
            objs.Add(GameScreen.Player);
            foreach (var obs in objs)
            {
                bool hasCollider = obs.HasCollider;
                if (hasCollider && Rectangle.Intersects(obs.Rectangle))
                {
                    var collision = Helper.RectangleCollision(Rectangle, obs.Rectangle);

                    if (collision == Helper.Collision.Top)
                    {
                        Position = new Vector2(Position.X, obs.Rectangle.Bottom + Rectangle.Height / 2);
                        CollidingTop = true;
                    }
                    else
                    {
                        CollidingTop = false;
                    }

                    if (collision == Helper.Collision.Bottom)
                    {
                        Position = new Vector2(Position.X, obs.Rectangle.Top - Rectangle.Height / 2);
                        CollidingBot = true;
                    }
                    else
                    {
                        CollidingBot = false;
                    }

                    if (collision == Helper.Collision.Right)
                    {
                        Position = new Vector2(obs.Rectangle.Left - Rectangle.Width / 2, Position.Y);
                        CollidingRight = true;
                    }
                    else
                    {
                        CollidingRight = false;
                    }

                    if (collision == Helper.Collision.Left)
                    {
                        Position = new Vector2(obs.Rectangle.Right + Rectangle.Width / 2, Position.Y);

                        CollidingLeft = true;
                    }
                    else
                    {
                        CollidingLeft = false;
                    }
                }
                else
                {
                    CollidingBot = false;
                    CollidingTop = false;
                    CollidingLeft = false;
                    CollidingRight = false;
                }
            }
        }

        protected void CheckHealth()
        {
            if (Health <= 0)
            {
                Dead = true;
            }
        }

        protected void DrawHealthBar()
        {
            int width = 60;
            int currentWidth = (int)Helper.MapValue(Health, maxHealth, 0, width, 0);
            int h = 10;
            var mainRect = Helper.RectangleFromPosition(new Vector2(Position.X, Position.Y - Rectangle.Width), width, h);
            var currentRect = new Rectangle(mainRect.X, mainRect.Y, currentWidth, h);
            Game.SpriteBatch.Draw(Game.Assets.Rectangle, mainRect, Color.White);
            Game.SpriteBatch.Draw(Game.Assets.Rectangle, currentRect, Color.Green);
        }
    }
}
