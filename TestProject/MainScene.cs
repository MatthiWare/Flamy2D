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
        private Fire f;
        private Fire f2;
        private Fire f3;

        public MainScene()
            : base(ID)
        {
            p = new Player();
            f = new Fire(10, 10);
            f2 = new Fire(-100, -10);
            f3 = new Fire(700, 400);
        }


        public override void Update(Game game)
        {
            f.Update(game);
            f2.Update(game);
            f3.Update(game);

            p.Update(game);
        }

        public override void Render(Game game, SpriteBatch batch)
        {
            f.Render(game, batch);
            f2.Render(game, batch);
            f3.Render(game, batch);

            p.Render(game, batch);
        }

        
    }
}
