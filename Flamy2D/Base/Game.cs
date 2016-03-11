using Flamy2D;
using Flamy2D.Extensions;
using OpenTK;
using OpenTK.Graphics;
using Flamy2D.Input;
using System.Threading;
using OpenTK.Graphics.OpenGL4;
using Flamy2D.Graphics;
using Flamy2D.Assets;

namespace Flamy2D.Base
{
    /// <summary>
    /// The GameEngine containing all the game logic.
    /// </summary>
    public class Game : ILog
    {

        public ContentManager Content { get; private set; }

        /// <summary>
        /// The game configuration. 
        /// </summary>
        public GameConfiguration Configuration { get; private set; }

        /// <summary>
        /// The resolution of the game. 
        /// </summary>
        public Resolution Resolution { get; set; }

        /// <summary>
        /// The Keyboard. 
        /// </summary>
        public Keyboard Keyboard { get; set; }

        public GameTime Time { get; private set; }

        public bool Closing { get; private set; }

        private NativeWindow window;
        private GraphicsContext context;
        private GraphicsMode graphicsMode;
        
        private bool updating = false;

        private SpriteBatch batch;

        // Timings data
        private double updatesPerSec;

        public Game(GameConfiguration config)
        {
            Configuration = config;
            Closing = false;

            Keyboard = new Keyboard();
             Time = new GameTime();

            Content = new ContentManager();
        }

        /// <summary>
        /// Runs the game. 
        /// </summary>
        public void Run()
        {
            Initialize();

            Thread gameloopThread = new Thread(GameLoop);
            gameloopThread.Name = "Game Loop Thread";
            gameloopThread.Start();

            this.Log("Enter message processing loop");
            while (!Closing && window != null && window.Exists)
            {
                window.ProcessEvents();
            }
            this.Log("Exited message processing loop");

            this.Log("Waiting for game to finish");
            while (updating) ;


        }

        private void GameLoop()
        {
            this.Log("Create graphics context");

            context = new GraphicsContext(graphicsMode, window.WindowInfo);

            context.MakeCurrent(window.WindowInfo);

            GraphicsContext.Assert();

            this.Log("Enable VSync: {0}", Configuration.VSync);
            if (Configuration.VSync)
                context.SwapInterval = 1;
            else
                context.SwapInterval = 0;

            this.Log("Loading OpenGL entry points");
            context.LoadAll();

            batch = new SpriteBatch(this);

            this.Log("Setup OpenGL");
            SetupOpenGL();

            window.Visible = true;

            CalculateTimings();

            Time.Start();

            this.Log("Enter game loop");
            while (!Closing)
            {
                if (context.IsDisposed)
                {
                    this.Log("Context not available");
                    break;
                }

                updating = true; 

                InternUpdate();
                Render(batch);

                updating = false;
            }
            this.Log("Exited game loop");

            Time.Stop();
        }

        private void CalculateTimings()
        {
            updatesPerSec = 1.0 / (double)Configuration.FPSTarget;
        }

        private double frameDelta;
        private void InternUpdate()
        {
            frameDelta = Time.Update();

            Update();

            while (Configuration.FixedFPS && frameDelta < updatesPerSec)
            {
                Update();
                frameDelta += Time.Update();
            }

            frameDelta = 0;
        }

        protected virtual void Update() { }
        protected virtual void Render(SpriteBatch batch)
        {
            if (!context.IsDisposed)
                context.SwapBuffers();
        }

        protected virtual void SetupOpenGL()
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
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

            this.Log("Creating native window");
            window = new NativeWindow(
                width: Configuration.Width,
                height: Configuration.Height,
                title: Configuration.Title,
                options: flags,
                mode: graphicsMode,
                device: DisplayDevice.Default
            );

            window.Icon = Flamy2D.Properties.Resources.Flamy;

            updateResolution();

            // adding window resize event handler
            window.Resize += (o, e) => OnResize();

            // adding keyboard event handler
            window.KeyDown += (o, e) => Keyboard.RegisterKeyDown(o, e);
            window.KeyUp += (o, e) => Keyboard.RegisterKeyUp(o, e);

            window.Closing += (o, e) =>
            {
                this.Log("Window is closing");
                Exit();

                // else the window will instantly close and not wait for the game to properly exit. 
                e.Cancel = true;
            };

        }

        public void Exit()
        {
            OnExit();

            Closing = true;
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

            if (context != null && !context.IsDisposed)
                context.Update(window.WindowInfo);
        }

        protected virtual void OnExit()
        {

        }

    }
}
