using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using System;

namespace SlashySam
{
    public static class Helper
    {
        public static void Init()
        {
            SquareTexture.SetData(new Color[] { Color.White });
        }

        public static Texture2D SquareTexture { get; } = new Texture2D(Game.Graphics.GraphicsDevice, 1, 1);

        public static Random Rand
        {
            get;
        } = new Random();

        public static Vector2 MouseInWorld
        {
            get
            {
                // Get mouse position on screen
                Vector2 mouse = Mouse.GetState().Position.ToVector2();

                // Convert it to world position
                var mouseInWorld = Vector2.Zero;
                Game.Camera.ToWorld(ref mouse, out mouseInWorld);

                // Apply offset
                mouseInWorld.X -= (Game.Screen.X * Game.Camera.Transform.Scale.X) / 2;
                mouseInWorld.Y -= (Game.Screen.Y * Game.Camera.Transform.Scale.Y) / 2;

                return mouseInWorld;
            }
        }

        public static Vector2 RandomPointOnCircle(Vector2 position, int radius)
        {

            int randomAngle = Rand.Next(360);
            float angleToRadians = MathHelper.ToRadians(randomAngle);

            var x = position.X + (MathF.Cos(angleToRadians) * radius);
            var y = position.Y + (MathF.Sin(angleToRadians) * radius);

            return new Vector2(x, y);
        }

        // Creates a rectenagle with the center to a specified position
        public static Rectangle RectangleFromPosition(Vector2 position, int width, int height)
        {
            return new Rectangle((int)(position.X - width / 2), (int)(position.Y - height / 2), width, height);
        }
        public static Rectangle RectangleFromPosition(Vector2 position, int size)
        {
            return new Rectangle((int)(position.X - size / 2), (int)(position.Y - size / 2), size, size);
        }
        // Check if Vector2 in Rectangle
        public static bool Includes(this Rectangle rectangle, Vector2 position)
        {
            return position.X > rectangle.Left && position.X < rectangle.Right && position.Y > rectangle.Top && position.Y < rectangle.Bottom;
        }
        public static bool Includes(this Rectangle rectangle, Point position)
        {
            return position.X > rectangle.Left && position.X < rectangle.Right && position.Y > rectangle.Top && position.Y < rectangle.Bottom;
        }
        public static Vector2 RandomVectorInside(this Rectangle rectangle)
        {
            return new Vector2(Rand.Next(rectangle.Left, rectangle.Right), Rand.Next(rectangle.Top, rectangle.Bottom));
        }
        // Get scale for a texture given a desired Size
        public static Vector2 CreateScale(this Texture2D texture, int width, int height)
        {
            return new Vector2((float)width / (float)texture.Width, (float)height / (float)texture.Height);
        }
        public static Vector2 CreateScale(this Texture2D texture, int size)
        {
            return new Vector2((float)size / (float)texture.Width, (float)size / (float)texture.Height);
        }
        public static float RandomFloat(this Point point)
        {
            return (float)(Rand.NextDouble() * (point.Y - point.Y)) + point.X;
        }
        public static int RandomInt(this Point point)
        {
            return Rand.Next(point.X, point.Y);
        }
        public static float RandomFloat(float x, float y)
        {
            return (float)(Rand.NextDouble() * (y - x)) + x;
        }
        public static float RandomFloat(this Size point)
        {
            return (float)(Rand.NextDouble() * (point.Y - point.Y)) + point.X;
        }
        public static int RandomInt(this Size point)
        {
            if (point.X < point.Y) return Rand.Next((int)point.X, (int)point.Y);
            else return Rand.Next((int)point.Y, (int)point.X);
        }
        public static float MapValue(float value, float start1, float stop1, float start2, float stop2)
        {
            var outgoing = start2 + (stop2 - start2) * ((value - start1) / (stop1 - start1));
            return outgoing;
        }
        // Find angle between 2 vectors
        public static float Angle(Vector2 from, Vector2 to)
        {
            return (float)Math.Atan2(from.X - to.X, from.Y - to.Y);
        }

        static public void DrawString(this SpriteBatch spriteBatch, SpriteFont font, string strToDraw, Color color, Rectangle boundaries)
        {
            Vector2 size = font.MeasureString(strToDraw);

            float xScale = (boundaries.Width / size.X);
            float yScale = (boundaries.Height / size.Y);

            // Taking the smaller scaling value will result in the text always fitting in the boundaires.
            float scale = Math.Min(xScale, yScale);

            // Figure out the location to absolutely-center it in the boundaries rectangle.
            int strWidth = (int)Math.Round(size.X * scale);
            int strHeight = (int)Math.Round(size.Y * scale);
            Vector2 position = new Vector2();
            position.X = (((boundaries.Width - strWidth) / 2) + boundaries.X);
            position.Y = (((boundaries.Height - strHeight) / 2) + boundaries.Y);

            // A bunch of settings where we just want to use reasonable defaults.
            float rotation = 0.0f;
            Vector2 spriteOrigin = new Vector2(0, 0);
            float spriteLayer = 0.0f; // all the way in the front
            SpriteEffects spriteEffects = new SpriteEffects();

            // Draw the string to the sprite batch!
            spriteBatch.DrawString(font, strToDraw, position, color, rotation, spriteOrigin, scale, spriteEffects, spriteLayer);
        }

        public enum Collision
        {
            None,
            Top,
            Right,
            Left,
            Bottom
        }

        public static Collision RectangleCollision(Rectangle r1, Rectangle r2)
        {
            Collision result;

            float w = 0.5f * (r1.Width + r2.Width);
            float h = 0.5f * (r1.Height + r2.Height);
            float dx = r1.Center.X - r2.Center.X;
            float dy = r1.Center.Y - r2.Center.Y;

            if (r1.Intersects(r2))
            {

                // Collision
                float wy = w * dy;
                float hx = h * dx;

                if (wy > hx)
                {
                    if (wy > -hx)
                    {
                        result = Collision.Top;
                    }
                    else
                    {
                        result = Collision.Right;
                    }
                }
                else
                {
                    if (wy > -hx)
                    {
                        result = Collision.Left;
                    }
                    else
                    {
                        result = Collision.Bottom;
                    }
                }

            }
            else
            {
                result = Collision.None;
            }

            return result;
        }

        // Line drawing code taken from the Monogame Community website
        // - SpriteBatch Extension
        private static Texture2D _texture;
        private static Texture2D GetTexture(SpriteBatch spriteBatch)
        {
            if (_texture == null)
            {
                _texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                _texture.SetData(new[] { Color.White });
            }

            return _texture;
        }

        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness = 1f)
        {
            var distance = Vector2.Distance(point1, point2);
            var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            DrawLine(spriteBatch, point1, distance, angle, color, thickness);
        }

        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color, float thickness = 1f)
        {
            var origin = new Vector2(0f, 0.5f);
            var scale = new Vector2(length, thickness);
            spriteBatch.Draw(GetTexture(spriteBatch), point, null, color, angle, origin, scale, SpriteEffects.None, 0);
        }
    }
}
