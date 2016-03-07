using Flamy2D.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flamy2D.Scenes
{
    public abstract class GameScene
    {
        public String Name { get; protected set; }

        public GameScene(String name)
        {
            Name = name;
        }

        public abstract void Update(GameEngine game);

        public abstract void Render(GameEngine game, SpriteBatch batch);

        public virtual void SceneEntered()
        {

        }

        public virtual void SceneExited()
        {

        }
    }
}
