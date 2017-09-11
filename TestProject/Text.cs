using Flamy2D.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Flamy2D;
using Flamy2D.Graphics;
using Flamy2D.Fonts;
using OpenTK.Graphics;

namespace TestProject
{
    public class Text : GameObject
    {

        private float x, y, drawX, drawY;
        private string text;
        private BitmapFont font;

        public Text(string txt, BitmapFont f, float x, float y)
        {
            text = txt;
            font = f;
            this.x = x;
            this.y = y;
        }

        public override void Load(Game game)
        {
            Console.WriteLine("loaded");
        }

        public override void Update(Game game)
        {
            base.Update(game);

            TestGame g = (TestGame)game;

            drawX = x - g.camera.x;
            drawY = y - g.camera.y;
        }

        public override void Render(Game game, SpriteBatch batch)
        {
            font.DrawString(batch, text, (int)drawX, (int)drawY, Color4.White);
        }
    }
}
