using OpenTK;
using OpenTK.Graphics;
using Flamy2D.Input;
using System.Threading;
using OpenTK.Graphics.OpenGL4;
using Flamy2D.Graphics;
using Flamy2D.Assets;
using System;
using Flamy2D.Assets.Providers;
using Flamy2D.Audio;
using OpenTK.Audio;
using Flamy2D.Fonts;

namespace Flamy2D.Base
{
    /// <summary>
    /// The Game containing all the game logic.
    /// </summary>
    public class BaseGame : IDisposable
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

        public VSyncMode VSync
        {
            get
            {
                GraphicsContext.Assert();

                if (context.SwapInterval < 0)
                    return VSyncMode.Adaptive;
                else if (context.SwapInterval == 0)
                    return VSyncMode.Off;
                else
                    return VSyncMode.On;
            }
            set
            {
                GraphicsContext.Assert();

                switch (value)
                {
                    case VSyncMode.Adaptive:
                        context.SwapInterval = -1;
                        break;
                    case VSyncMode.On:
                        context.SwapInterval = 1;
                        break;
                    case VSyncMode.Off:
                        context.SwapInterval = 0;
                        break;
                }
            }
        }

        private NativeWindow window;
        private GraphicsContext context;
        private GraphicsMode graphicsMode;

        private bool updating = false;

        private SpriteBatch batch;

        // Timings data
        private double updatesPerSec;

        /// <summary>
        /// Initializes the static game
        /// See <see cref="SetUnmanagedDllDirectory"/> for more information. 
        /// </summary>
        static BaseGame()
        {
            //SetUnmanagedDllDirectory();
        }

        public BaseGame(GameConfiguration config)
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
            gameloopThread.Name = "BaseGame Loop Thread";
            gameloopThread.Start();

            this.Log("Enter message processing loop");
            while (!Closing && window != null && window.Exists)
            {
                window.ProcessEvents();
            }
            this.Log("Exited message processing loop");

            this.Log("Waiting for game to finish");
            while (updating) ;

            gameloopThread = null;

        }

        private void GameLoop()
        {
            this.Log("Create graphics context");

            context = new GraphicsContext(graphicsMode, window.WindowInfo);

            context.MakeCurrent(window.WindowInfo);

            GraphicsContext.Assert();

            this.Log($"VSync: {Configuration.VSync}");
            VSync = Configuration.VSync;

            this.Log("Loading OpenGL entry points");
            context.LoadAll();

            batch = new SpriteBatch(this);

            this.Log("Setup OpenGL");
            SetupOpenGL();

            this.Log("Setup OpenAL");
            SetupOpenAL();
            LoadAssetProviders();

            Load();

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

        public virtual void LoadAssetProviders()
        {
            Content.RegisterAssetHandler<Texture2D>(typeof(TextureProvider));
            Content.RegisterAssetHandler<Font>(typeof(FontProvider));
            Content.RegisterAssetHandler<Sound>(typeof(SoundProvider));
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

        protected virtual void Load() { }

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

        private void SetupOpenAL()
        {
            AudioContext ctx = new AudioContext();
            AudioDevice.Instance = new AudioDevice(updateRate: 2);
        }

        /// <summary>
        /// Pre <see cref="Run"/> Initializes the BaseGame. 
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

            GL.Viewport(0, 0, window.Width, window.Height);
        }

        protected virtual void OnExit()
        {

        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                Exit();

                if (updating)
                {
                    this.Log("Wait for the game to stop updating..");

                    while (updating) ;
                }


                if (disposing)
                {
                    context.Dispose();
                    window.Dispose();
                    batch.Dispose();
                    Keyboard.Dispose();
                    Content.Dispose();
                    Time.Dispose();
                }



                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~BaseGame() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
