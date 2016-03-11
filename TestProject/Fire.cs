using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Flamy2D;
using Flamy2D.Graphics;
using OpenTK.Graphics;
using OpenTK.Input;

namespace TestProject
{
    public class Fire
    {
        private float scale = 1f;
        private float x, y;
        private float drawX, drawY;
        private Texture2D[] textures;
        private Texture2D tex;
        private bool loaded = false;
        private  float flame_switch= 0.22f;
        private float flame_switch_timer = 0;
        private int index = 0;

        private static Random rnd = new Random();
        public Fire(float x, float y)
        {
            this.x = x;
            this.y = y;
            textures=new Texture2D[3];
            index = rnd.Next(0, 3);
        }

        public void Update(GameEngine game)
        {
            TestGame g = (TestGame) game;

            drawX = x - g.camera.x;
            drawY = y - g.camera.y;

            if (!loaded)
            {
                for (int i = 0; i < textures.Length; i++)
                {
                    textures[i]= Texture2D.LoadFromFile("../../../Content/TestProject/fire"+i+".png", TextureConfiguration.Nearest);
                }

                tex = textures[index];
                loaded = true;
            }

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

        public void Render(GameEngine game, SpriteBatch batch)
        {
            batch.Draw(tex, drawX, drawY, Color4.White, scale);
        }
    }
}
