

using System;
using System.Diagnostics;

namespace Flamy2D.Base
{
    /// <summary>
    /// GameTime contains the total time the game is running and the time since the last update (delta time).
    /// </summary>
    public class GameTime
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

    }
}
