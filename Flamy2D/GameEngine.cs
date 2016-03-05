using System;
using Flamy2D.Extensions;
using OpenTK;
using OpenTK.Graphics;
using Flamy2D.Input;

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

        /// <summary>
        /// The resolution of the game. 
        /// </summary>
        public Resolution Resolution { get; set; }

        public Keyboard Keyboard { get; set; }


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
            Initialize();
        }

        /// <summary>
        /// Pre <see cref="Run"/> Initializes the Game. 
        /// </summary>
        private void Initialize()
        {
            GameWindowFlags flags = GameWindowFlags.Default;

            if (Configuration.Fullscreen && !flags.HasFlag(GameWindowFlags.Fullscreen))
                flags |= GameWindowFlags.Fullscreen;

            if (!Configuration.Resizable && !flags.HasFlag(GameWindowFlags.Fullscreen))
                flags |= GameWindowFlags.FixedWindow;

            this.Log("Creating native window");
            window = new NativeWindow(
                width: Configuration.Width,
                height: Configuration.Height,
                title: Configuration.Title,
                options: flags,
                mode: GraphicsMode.Default,
                device: DisplayDevice.Default
            );

            window.Icon = Flamy2D.Properties.Resources.Flamy;

            updateResolution();

            window.Resize += (o, e) => OnResize();

            window.KeyDown += (o, e) => Keyboard.RegisterKeyDown(o, e);
            window.KeyUp += (o, e) => Keyboard.RegisterKeyUp(o, e);
        }

        /// <summary>
        /// Resizes the game. 
        /// </summary>
        /// <param name="w">The new width. </param>
        /// <param name="h">The new height. </param>
        public void Resize(int w, int h)
        {
            window.Width = w;
            window.Height = h;

            OnResize();
        }

        private void updateResolution()
        {
            Resolution = new Resolution(window.Width, window.Height);
        }

        protected virtual void OnResize()
        {
            updateResolution();


        }


    }
}
