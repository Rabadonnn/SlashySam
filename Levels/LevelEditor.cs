using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace SlashySam.Levels
{
    public class LevelEditor : Level
    {
        public override int ID => 0;
        void LoadMap()
        {
            using (var stream = new System.IO.FileStream("./Content/Levels/Level_01.png", System.IO.FileMode.Open))
            {
                Map.Load(Texture2D.FromStream(Game.Graphics.GraphicsDevice, stream));
                Objects.Clear();
                Objects.AddRange(Map.GetObstacles());
                Enemies.Clear();
                Enemies.AddRange(Map.GetEnemies());
            }
        }
        
        public override void Restart()
        {
            Map = new Map();
            LoadMap();
            var cf = new GameObjects.Env.Campfire(new Vector2(-100, 0));
        }

        bool canLoadMap = true;
        public override void Update()
        {
            UpdateObjects();
            if (Keyboard.GetState().IsKeyDown(Keys.L) && canLoadMap)
            {
                LoadMap();
                canLoadMap = false;
            }
            if (Keyboard.GetState().IsKeyUp(Keys.L) && !canLoadMap)
            {
                canLoadMap = true;
            }
        }

        public override void Draw()
        {
            Map.Draw();
            DrawObjects();
        }
    }
}
