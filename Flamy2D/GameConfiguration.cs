using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flamy2D
{
    public class GameConfiguration
    {
        /// <summary>
        /// If the game should start in Fullscreen (true) or not (false). 
        /// </summary>
        public bool Fullscreen { get; set; }

        /// <summary>
        /// If the game window should be resizable yes (true) or not (false). 
        /// </summary>
        public bool Resizable { get; set; }

        /// <summary>
        /// If the frames per second should be fixed to <see cref="FPSTarget"/>
        /// </summary>
        public bool FixedFPS { get; set; }

        /// <summary>
        /// If the game should use Vertical sync. 
        /// </summary>
        public VSyncMode VSync { get; set; }

        /// <summary>
        /// The width of the game window
        /// </summary>
        public int Width { get; set; }
        
        /// <summary>
        /// The height of the game window
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// The frame per second target.
        /// Only works if <see cref="FixedFPS"/> is enabled. 
        /// </summary>
        public int FPSTarget { get; set; }

        /// <summary>
        /// The game window title. 
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// .ctor for <see cref="GameConfiguration"/>. 
        /// </summary>
        public GameConfiguration()
        {
            Title = "Flamy 2D Engine";
            Width = 640;
            Height = 480;
            VSync = VSyncMode.Off;
            FPSTarget = 60;
            FixedFPS = false;
        }
    }
}
