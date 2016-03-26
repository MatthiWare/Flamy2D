

using System;
using System.Diagnostics;

namespace Flamy2D.Base
{
    /// <summary>
    /// GameTime contains the total time the game is running and the time since the last update (delta time).
    /// </summary>
    public class GameTime : IDisposable
    {
        private Stopwatch watch;

        /// <summary>
        /// The total time the game is running for in MS.
        /// </summary>
        public double Total { get { return watch.ElapsedMilliseconds * 0.001; } }

        /// <summary>
        /// The delta time in MS. 
        /// </summary>
        public double Elapsed { get; private set; }

        private double lastUpdated;

        /// <summary>
        /// .ctor for <see cref="Flamy2D.GameTime"/>
        /// </summary>
        public GameTime()
        {
            Elapsed = 0;
            lastUpdated = 0;
            watch = new Stopwatch();
        }

        public void Start()
        {
            watch.Start();
        }

        public void Stop()
        {
            watch.Stop();
        }

        public double Update()
        {
            double now = Total;
            Elapsed = now - lastUpdated;
            lastUpdated = now;
            return Elapsed;

        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                Stop();
                watch = null;

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~GameTime() {
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
