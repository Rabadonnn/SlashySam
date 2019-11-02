using Microsoft.Xna.Framework;

namespace SlashySam.GameObjects.Player
{
    public class Shuriken : GameObject
    {
        Vector2 direction;
        float speed = 1000;
        float lifetime = 2f;

        public Shuriken(Vector2 position, Vector2 _direction)
        {
            Position = position;
            direction = _direction;
            Texture = Game.Assets.Player.Shuriken;
            Scale = new Vector2(1.6f);
        }

        protected override void UpdateMyself()
        {
            direction.Normalize();
            direction *= speed * Game.DeltaTime;
            Position += direction;
            Rotation += System.MathF.PI / 180;
            lifetime -= Game.DeltaTime;

            if (lifetime < 0)
                Dead = true;

            foreach (var obs in GameScreen.CurrentLevel.Objects)
            {
                bool hasCollider = obs.HasCollider;
                if (hasCollider && Rectangle.Intersects(obs.Rectangle))
                {
                    Dead = true;
                }
            }
        }
    }
}
