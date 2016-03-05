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

        public virtual void Update()
        {

        }

        public virtual void Render()
        {

        }
    }
}
