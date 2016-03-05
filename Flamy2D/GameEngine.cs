using OpenTK;
using System;

namespace Flamy2D
{
    /// <summary>
    /// The GameEngine containing all the game logic.
    /// </summary>
    public class GameEngine
    {
        /// <summary>
        /// The game configuration. 
        /// </summary>
        public GameConfiguration Configuration { get; private set; }

        private NativeWindow window;

        public GameEngine(GameConfiguration config)
        {
            Configuration = config;
        }

        /// <summary>
        /// Runs the game. 
        /// </summary>
        public void Run()
        {

        }


    }
}
