using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SlashySam.UI
{
    public class PopUpMenu
    {
        public List<GameObject> Components
        {
            get;
        } = new List<GameObject>();

        public Rectangle Rectangle
        {
            get; private set;
        }

        public Color Color { get; set; } = Color.White;
        public Texture2D Texture { get; set; } = Game.Assets.Rectangle;

        public PopUpMenu(Rectangle rectangle)
        {
            Rectangle = rectangle;
        }

        public void Update()
        {
            foreach (var component in Components)
            {
                component.Update();
            }
        }

        public void Draw()
        {
            Game.SpriteBatch.Draw(Texture, Rectangle, Color);

            foreach (var component in Components)
            {
                component.Draw();
            }
        }
    }
}
