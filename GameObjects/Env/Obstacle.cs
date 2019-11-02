using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SlashySam.GameObjects.Env
{
    public class Obstacle : GameObject
    {
        // Default constructor
        public Obstacle(Texture2D texture, Vector2 position, Vector2 scale)
        {
            HasCollider = true;
            Texture = texture;
            Position = position;
            Scale = scale;
        }
        // Constructor for non-colliding objects
        public Obstacle(Texture2D texture, Vector2 position, Vector2 scale, bool hasCollider)
        {
            Texture = texture;
            Position = position;
            Scale = scale;
            HasCollider = hasCollider;
        }
    }
}
