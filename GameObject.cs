using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SlashySam
{
    public abstract class GameObject
    {

        // Default variables;

        public bool Enabled { get; set; } = true;
        public bool HasCollider { get; set; } = false;

        public Texture2D Texture
        {
            get;
            set;
        }
        public Vector2 Position
        {
            get;
            set;
        } = new Vector2();
        public Vector2 Scale
        {
            get; set;
        } = new Vector2(1, 1);
        public float Rotation
        {
            get;
            set;
        } = 0;
        // Returns a rectangle around the current player Position
        public virtual Rectangle Rectangle
        {
            get
            {
                // if there is no source rectangle, return a rectangle around player position based on texture properties
                if (sourceRectangle == null)
                {
                    return new Rectangle((int)(Position.X - (Texture.Width * Scale.X) / 2), (int)(Position.Y - (Texture.Height * Scale.Y) / 2), (int)(Texture.Width * Scale.X), (int)(Texture.Height * Scale.Y));
                }
                // Else return one based on source rectangle details
                else
                {
                    var rect = new Rectangle()
                    {
                        X = (int)(Position.X - (sourceRectangle.Value.Width * Scale.X) / 2),
                        Y = (int)(Position.Y - (sourceRectangle.Value.Height * Scale.Y) / 2),
                        Width = (int)(sourceRectangle.Value.Width * Scale.X),
                        Height = (int)(sourceRectangle.Value.Height * Scale.Y)
                    };

                    return rect;
                }
            }
        }
        protected Rectangle? sourceRectangle
        {
            get;
            set;
        } = null;
        public bool Dead
        {
            get;
            set;
        } = false;
        public Color Color
        {
            get;
            set;
        } = Color.White;
        protected virtual Vector2 Origin
        {
            get
            {
                return new Vector2(Texture.Width / 2, Texture.Height / 2);
            }
        }
        public SpriteEffects SpriteEffects
        {
            get;
            set;
        } = SpriteEffects.None;
        protected void DefaultDraw()
        {
            Game.SpriteBatch.Draw(Texture, Position, sourceRectangle, Color, Rotation, Origin, Scale, this.SpriteEffects, 0);
        }
        protected virtual void UpdateMyself()
        {

        }
        protected virtual void DrawMyself()
        {
            DefaultDraw();
        }

        public void Update()
        {
            if (Enabled)
            {
                UpdateMyself();
            }
        }
        public void Draw()
        {
            if (Enabled)
            {
                DrawMyself();
            }
        }
    }
}
