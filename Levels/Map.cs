using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SlashySam.GameObjects.Enemies;
using SlashySam.GameObjects.Env;
using System.Collections.Generic;

namespace SlashySam.Levels
{
    public class Map
    {
        public Texture2D Texture { get; set; }
        public Rectangle Rectangle { get; private set; }
        private Tile[] tiles;

        public Map() { }
        public Map(Texture2D texture)
        {
            Load(texture);
        }
        private List<GameObject> Obstacles { get; } = new List<GameObject>();
        private List<Enemy> Enemies { get; } = new List<Enemy>();

        public void Load(Texture2D texture)
        {
            Obstacles.Clear();
            Enemies.Clear();
            Texture = texture;
            Color[] data = new Color[Texture.Width * Texture.Height];
            Texture.GetData<Color>(data);

            var bruteTiles = new Tile[texture.Width * texture.Height];

            int tileCounter = 0;
            for (int i = 0; i < Texture.Height; i++)
            {
                for (int j = 0; j < Texture.Width; j++)
                {
                    var currentPixelColor = data[i * texture.Width + j];
                    var t = new Tile(this, new Vector2(j, i), currentPixelColor);
                    if (t.Valid)
                    {
                        bruteTiles[tileCounter] = t;
                        tileCounter++;
                    }
                }
            }

            tiles = new Tile[tileCounter];
            for (int i = 0; i < tileCounter; i++)
            {
                tiles[i] = bruteTiles[i];
            }

            Rectangle = new Rectangle(-Texture.Width * Tile.TILE_WIDTH / 2, -Texture.Height * Tile.TILE_WIDTH / 2, Texture.Width * Tile.TILE_WIDTH, Texture.Height * Tile.TILE_WIDTH);
        }

        public List<GameObject> GetObstacles()
        {
            return Obstacles;
        }
        public List<Enemy> GetEnemies()
        {
            return Enemies;
        }

        public void Draw()
        {
            var bounds = new Rectangle(Game.CameraBounds.X - 50, Game.CameraBounds.Y - 50, Game.CameraBounds.Width + 100, Game.CameraBounds.Height + 100);
            for (int i = 0; i < tiles.Length; i++)
            {
                if (tiles[i] != null && bounds.Includes(tiles[i].Position))
                {
                    tiles[i].Draw();
                }
            }
        }

        class Tile
        {
            public Vector2 Position { get; }
            Color PixelColor { get; set; }
            Color Color { get; set; } = Color.White;

            public const int TILE_WIDTH = 32;
            Texture2D Texture { get; set; }

            Rectangle Rectangle { get; set; }
            Rectangle? SourceRectangle { get; set; } = null;
            float Rotation { get; set; } = 0;
            public bool Valid { get; private set; } = true;
            SpriteEffects SpriteEffects { get; set; } = SpriteEffects.None;
            Map Map { get; }
            Vector2 Origin { get; set; } = Vector2.Zero;

            public Tile(Map map, Vector2 position, Color color)
            {
                PixelColor = color;
                position *= TILE_WIDTH;
                position.X -= map.Texture.Width * TILE_WIDTH / 2;
                position.Y -= map.Texture.Height * TILE_WIDTH / 2;
                Rectangle = Helper.RectangleFromPosition(position, TILE_WIDTH, TILE_WIDTH);
                Texture = Helper.SquareTexture;
                Position = Rectangle.Center.ToVector2();
                Map = map;
                if (CreatesObject()) Valid = false;
                else ChooseTexture();
            }

            public static Color RandomUnWalkable => new Color(20, 0, 208);
            static Color RandomWalkable => new Color(255, 0, 96);
            static Color House => new Color(2, 0, 16);
            static Color SquarePath => new Color(100, 100, 100);
            static Color Scout => new Color(255, 0, 0);
            static Color Box => new Color(153, 100, 62);


            bool Equals(Color color)
            {
                return PixelColor.R == PixelColor.R && PixelColor.G == color.G && PixelColor.B == color.B;
            }

            bool CreatesObject()
            {
                if (Equals(RandomUnWalkable))
                {
                    int rand = Helper.Rand.Next(5);
                    Obstacle obs;
                    var color = Helper.Rand.Next(2) == 1 ? Color.Gray : Color.DarkGray;
                    if (rand == 0)
                    {
                        float scale = Helper.RandomFloat(1.6f, 2.6f);
                        obs = new Obstacle(Game.Assets.Env.Bush_01, Position, new Vector2(scale)) { Color = color };
                    }
                    else if (rand == 1)
                    {
                        float scale = Helper.RandomFloat(3, 5f);
                        obs = new Obstacle(Game.Assets.Env.Dead_Tree, Position, new Vector2(scale)) { Color = color };
                    }
                    else if (rand == 2)
                    {
                        float scale = Helper.RandomFloat(2.4f, 3f);
                        obs = new Obstacle(Game.Assets.Env.Rock_01, Position, new Vector2(scale)) { Color = color };
                    }
                    else
                    {
                        float scale = Helper.RandomFloat(2.2f, 3f);
                        obs = new Obstacle(Game.Assets.Env.Log_01, Position, new Vector2(scale)) { Color = color };
                    }

                    if (Helper.Rand.Next(2) == 1) obs.SpriteEffects = SpriteEffects.FlipHorizontally;
                    Map.Obstacles.Add(obs);
                    return true;
                }
                else if (Equals(House))
                {
                    Obstacle obs;

                    float scale = Helper.RandomFloat(5, 7);
                    Texture2D texture = Game.Assets.Env.House_01;
                    if (Helper.Rand.Next(2) == 0) texture = Game.Assets.Env.House_02;
                    obs = new Obstacle(texture, Position, new Vector2(scale));

                    if (Helper.Rand.Next(2) == 1) obs.SpriteEffects = SpriteEffects.FlipHorizontally;
                    Map.Obstacles.Add(obs);
                    return true;
                }
                else if (Equals(Scout))
                {
                    Map.Enemies.Add(new Scout(Position));
                    return true;
                }
                else if (Equals(Box))
                {
                    Map.Obstacles.Add(new Breakable(Position) { Color = Color.Gray });
                    return true;
                }
                return false;
            }

            void ChooseTexture()
            {
                if (Equals(SquarePath))
                {
                    Texture = Helper.SquareTexture;
                    Rectangle = Helper.RectangleFromPosition(Position, TILE_WIDTH);
                    Color = Helper.Rand.Next(100) > 15 ? new Color(103, 94, 110) : new Color(106, 99, 112);
                }
                else if (Equals(RandomWalkable))
                {
                    int rand = Helper.Rand.Next(4);
                    int size = Helper.Rand.Next(TILE_WIDTH, (int)(TILE_WIDTH * 1.5f));

                    if (rand == 0)
                    {
                        Texture = Game.Assets.Env.Flowers_01;
                        Color = Color.MediumVioletRed;
                        if (Helper.Rand.Next(2) == 1) Color = Color.BlueViolet;
                    }
                    else if (rand == 1)
                    {
                        Texture = Game.Assets.Env.Flowers_02;
                    }
                    else if (rand == 2)
                    {
                        Texture = Game.Assets.Env.Sand_Steps;
                    }
                    else if (rand == 3)
                    {
                        Texture = Game.Assets.Env.GrassSet;
                        SourceRectangle = new Rectangle(5 * Helper.Rand.Next(3), 0, 5, 6);
                        size = (int)Helper.RandomFloat(TILE_WIDTH / 1.8f, TILE_WIDTH / 1.2f);
                        int r = Helper.Rand.Next(3);
                        if (r == 0) Color = Color.Green;
                        else if (r == 2) Color = Color.DarkGreen;
                        else Color = Color.DarkSeaGreen;
                    }

                    if (Helper.Rand.Next(2) == 1) SpriteEffects = SpriteEffects.FlipHorizontally;

                    Rectangle = Helper.RectangleFromPosition(Position, size);
                }
                else if (Equals(Color.White))
                {
                    int rand = Helper.Rand.Next(10);
                    Texture = Game.Assets.Env.Ground;
                    if (rand < 8)
                    {
                        Valid = false;
                    }
                    else
                    {
                        SourceRectangle = new Rectangle(Helper.Rand.Next(0, 7) * 16, 0, 16, 16);
                        Rotation = MathHelper.ToRadians(Helper.Rand.Next(360));
                        Origin = new Vector2(8);
                        Rectangle = Helper.RectangleFromPosition(Position, (int)Helper.RandomFloat(TILE_WIDTH * 0.7f, TILE_WIDTH * 1.5f));
                    }
                }
                else
                {
                    Valid = false;
                }
            }

            public void Draw()
            {
                Game.SpriteBatch.Draw(Texture, Rectangle, SourceRectangle, Color, Rotation, Origin, SpriteEffects, 0);
            }
        }
    }
}
