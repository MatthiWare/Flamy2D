

using System;

namespace Flamy2D.Base
{
    /// <summary>
    /// GameTime contains the total time the game is running and the time since the last update (delta time).
    /// </summary>
    public class GameTime
    {

        /// <summary>
        /// The total time the game is running for. 
        /// <see cref="System.TimeSpan"/>
        /// </summary>
        public TimeSpan Total { get; private set; }

        /// <summary>
        /// The delta time (time since last update). 
        /// <see cref="System.TimeSpan"/>
        /// </summary>
        public TimeSpan Elapsed { get; private set; }


        /// <summary>
        /// .ctor for <see cref="Flamy2D.GameTime"/>
        /// </summary>
        public GameTime()
        {
            Total = new TimeSpan();
            Elapsed = new TimeSpan();
        }

    }
}
