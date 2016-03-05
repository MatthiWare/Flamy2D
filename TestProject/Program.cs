using Flamy2D;
using System;

namespace TestProject
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            GameConfiguration config = new GameConfiguration();

            TestGame game = new TestGame(config);
            game.Run();
        }
    }
}
