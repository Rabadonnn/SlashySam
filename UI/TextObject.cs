using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SlashySam.UI
{
    public class TextObject : GameObject
    {
        public string Text { get; set; } = "";
        public SpriteFont Font { get; set; } = Game.Assets.ThaleahFont;

        Vector2 TextProps
        {
            get
            {
                return Font.MeasureString(Text);
            }
        }

        public override Rectangle Rectangle
        {
            get
            {
                int width = (int)(TextProps.X * Scale.X);
                int height = (int)((TextProps.Y * Scale.Y) / 2);
                return Helper.RectangleFromPosition(Position, width, height);
            }
        }

        protected override Vector2 Origin
        {
            get {
                return new Vector2(TextProps.X / 2, TextProps.Y / 2);
            }
        }

        public TextObject(Vector2 position, Vector2 scale, string text)
        {
            Position = position;
            Scale = scale;
            Text = text;
        }

        protected override void DrawMyself()
        {
            Game.SpriteBatch.DrawString(Font, Text, Position, Color, Rotation, Origin, Scale, SpriteEffects.None, 0);
        }
    }
}
