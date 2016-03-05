using System;
using Flamy2D.Base;
using Flamy2D.Scenes;

namespace Flamy2D
{
    public class GameEngine : Game
    {
        public GameSceneCollection Scenes { get; private set; }
        public GameScene CurrentScene { get; private set; }

        public GameEngine(GameConfiguration config)
            : base(config)
        {
            Scenes = new GameSceneCollection();
        }

        override 
    }
}
