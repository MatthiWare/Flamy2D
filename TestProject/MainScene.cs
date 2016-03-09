using Flamy2D;
using Flamy2D.Graphics;
using Flamy2D.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestProject
{
    public class MainScene : GameScene
    {
        public static string ID = "MAINSCENE_ID";
        private Player p;
        public MainScene()
            : base(ID)
        {
            p = new Player();
        }

        public override void Update(GameEngine game)
        {
            p.Update(game);
        }

        public override void Render(GameEngine game, SpriteBatch batch)
        {
            p.Render(game, batch);
        }

        
    }
}
