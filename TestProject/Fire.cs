using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Flamy2D;
using Flamy2D.Graphics;
using OpenTK.Graphics;
using Flamy2D.GameObjects;

namespace TestProject
{
    public class Fire : GameObject
    {
        private float scale = 1f;
        private float x, y;
        private float drawX, drawY;
        private Texture2D[] textures;
        private Texture2D tex;
        private  float flame_switch= 0.22f;
        private float flame_switch_timer = 0;
        private int index = 0;

        private static Random rnd = new Random();
        public Fire(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public override void Load(Game game)
        {
            
            textures = new Texture2D[3];
            index = rnd.Next(0, 3);

            for (int i = 0; i < textures.Length; i++)
            {
                textures[i] =game.Content.Load<Texture2D>("fire" + i + ".png", TextureConfiguration.Nearest);
            }

            tex = textures[index];
        }

        public override void Update(Game game)
        {
            base.Update(game);

            TestGame g = (TestGame) game;

            drawX = x - g.camera.x;
            drawY = y - g.camera.y;

            flame_switch_timer += (float)g.Time.Elapsed;
            if (flame_switch_timer > flame_switch)
            {
                index += 1;
                if (index > 2)
                    index = 0;

                flame_switch_timer = 0;

                tex = textures[index];
            }
        }

        public override void Render(Game game, SpriteBatch batch)
        {
            batch.Draw(tex, drawX, drawY, Color4.White, scale);
        }
    }
}
