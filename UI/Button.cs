using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using System;

namespace SlashySam.UI
{
    public class Button : GameObject
    {
        public string text;
        public Color color = Color.White;
        public Color hoverColor = Color.Purple;
        public SpriteFont font = Game.Assets.ThaleahFont;
        public override Rectangle Rectangle
        {
            get
            {
                int width = (int)(textProps.X * scale);
                int height = (int)((textProps.Y * scale) / 2);
                if (!alignLeft) return Helper.RectangleFromPosition(Position, width, height);
                else return new Rectangle((int)Position.X, (int)(Position.Y - height / 2), width, height);
            }
        }
        float scale;
        Vector2 textProps;

        public Action action;
        public Action onHoverAction = delegate { };

        public bool alignLeft = false;

        public Button(Vector2 _position, float _scale, string _text, Action _function)
        {
            Position = _position;
            text = _text;
            scale = _scale;
            action = _function;
        }

        bool Hover
        {
            get
            {
                var mouse = Mouse.GetState().Position;
                if (Rectangle.Includes(mouse))
                {
                    return true;
                }
                return false;
            }
        }

        MouseState previousState;

        protected override void UpdateMyself()
        {
            var mouse = Mouse.GetState();
            if (mouse.LeftButton == ButtonState.Released && previousState.LeftButton == ButtonState.Pressed && Hover)
            {
                action();
            }
            previousState = mouse;

            onHoverAction();
        }
        Vector2 origin;
        protected override void DrawMyself()
        {
            textProps = font.MeasureString(text);

            if (!alignLeft) origin = new Vector2(textProps.X / 2, textProps.Y / 2);
            else origin = new Vector2(0, textProps.Y / 2);

            if (!Hover)
                Game.SpriteBatch.DrawString(font, text, Position, color, 0, origin, scale, SpriteEffects.None, 0);
            else
                Game.SpriteBatch.DrawString(font, text, Position, hoverColor, 0, origin, scale, SpriteEffects.None, 0);
        }

        string onHoverChar = ">";
        public void OnHoverAction()
        {
            hoverColor = Color.White;
            if (Hover && !text.StartsWith(onHoverChar)) text = onHoverChar + text;
            else if (!Hover && text.StartsWith(onHoverChar)) text = text.Substring(1);
        }
    }
}
