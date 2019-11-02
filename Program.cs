using System;

namespace SlashySam
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Game())
            {
                game.Run();
                if (Exit)
                {
                    game.Exit();
                }
            }
        }

        public static bool Exit
        {
            get; set;
        }
    }
}
