using System;
using Flamy2D.Extensions;
using OpenTK;
using OpenTK.Graphics;

namespace Flamy2D
{
    /// <summary>
    /// The GameEngine containing all the game logic.
    /// </summary>
    public class GameEngine : ILog
    {
        /// <summary>
        /// The game configuration. 
        /// </summary>
        public GameConfiguration Configuration { get; private set; }

        private NativeWindow window;
        private GraphicsMode graphicsMode;

        public GameEngine(GameConfiguration config)
        {
            Configuration = config;
        }

        /// <summary>
        /// Runs the game. 
        /// </summary>
        public void Run()
        {
            Initialize();
        }

        /// <summary>
        /// Pre <see cref="Run"/> Initializes the Game. 
        /// </summary>
        private void Initialize()
        {
            graphicsMode = GraphicsMode.Default;

            GameWindowFlags flags = GameWindowFlags.Default;

            if (Configuration.Fullscreen && !flags.HasFlag(GameWindowFlags.Fullscreen))
                flags |= GameWindowFlags.Fullscreen;

            if (!Configuration.Resizable && !flags.HasFlag(GameWindowFlags.Fullscreen))
                flags |= GameWindowFlags.FixedWindow;
        }


    }
}
