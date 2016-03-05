using System;

namespace Flamy2D
{
    /// <summary>
    /// Contains the resolution information of the <see cref="GameEngine"/> 
    /// </summary>
    public class Resolution
    {
        /// <summary>
        /// The width
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// The height
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// .ctor for <see cref="Resolution"/>
        /// </summary>
        /// <param name="w">The width of the window.</param>
        /// <param name="h">The height of the window.</param>
        public Resolution(int w, int h)
        {
            Width = w;
            Height = h;
        }

        /// <summary>
        /// Gets the aspect ratio of the game. 
        /// </summary>
        public float AspectRatio { get { return (float)Width / (float)Height; } }
    }
}
