using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SlashySam.UI
{
    public class PopUp : GameObject
    {
        float i_lifetime;
        Vector2 i_scale;
        float lifetime = 1.5f;
        float alpha = 1;
        public bool decreaseAlpha = true;
        public bool decreaseSize = false;
        public bool goUp = true;
        public int upSpeed = 100;
        public PopUp(Vector2 position, Vector2 scale, Texture2D texture, float _lifetime)
        {
            Position = position;
            Scale = scale;
            Texture = texture;
            lifetime = _lifetime;
            i_lifetime = _lifetime;
            i_scale = scale;
        }

        protected override void UpdateMyself()
        {
            if (decreaseAlpha)
            {
                alpha = Helper.MapValue(lifetime, i_lifetime, 0, 1, 0);
            }
            if (decreaseSize)
            {
                Scale = new Vector2(Helper.MapValue(lifetime, i_lifetime, 0, i_scale.X, 0), Helper.MapValue(lifetime, i_lifetime, 0, i_scale.Y, 0));
            }
            if (goUp)
            {
                Position = new Vector2(Position.X, Position.Y - Game.DeltaTime * upSpeed);
            }
            lifetime -= Game.DeltaTime;
            if (lifetime < 0)
            {
                Dead = true;
            }
        }

        protected override void DrawMyself()
        {
            Game.SpriteBatch.Draw(Texture, Position, sourceRectangle, Color * alpha, Rotation, Origin, Scale, this.SpriteEffects, 0);
        }
    }
}
